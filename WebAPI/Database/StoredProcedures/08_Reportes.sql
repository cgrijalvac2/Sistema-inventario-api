/* =====================================================================
   08 - CONSULTAS Y REPORTES: existencias, kardex, movimientos y dashboard
   ===================================================================== */

-- Listado de existencias
CREATE OR ALTER PROCEDURE sp_Inventario_Existencias
    @ID_Almacen   INT = NULL,
    @ID_Categoria INT = NULL,
    @Buscar       NVARCHAR(100) = NULL,
    @SoloConStock BIT = 0,
    @SoloBajo     BIT = 0
AS
BEGIN
    SET NOCOUNT ON;
    SELECT i.ID_Almacen, a.Nombre AS Almacen, i.ID_Producto, p.CodigoProducto, p.NombreProducto,
           c.NombreCategoria AS Categoria, u.Abreviatura AS UnidadMedida,
           i.StockActual, i.StockMinimo, i.StockMaximo, p.Costo,
           CAST(i.StockActual * p.Costo AS DECIMAL(18,2)) AS ValorInventario,
           CASE WHEN i.StockMinimo > 0 AND i.StockActual <= i.StockMinimo THEN N'BAJO'
                WHEN i.StockMaximo > 0 AND i.StockActual > i.StockMaximo THEN N'EXCESO'
                ELSE N'NORMAL' END AS Alerta
      FROM InventarioAlmacen i
      JOIN Almacen a ON a.ID_Almacen = i.ID_Almacen
      JOIN Producto p ON p.ID_Producto = i.ID_Producto
      JOIN Categoria c ON c.ID_Categoria = p.ID_Categoria
      JOIN UnidadMedida u ON u.ID_UnidadMedida = p.ID_UnidadMedida
     WHERE (@ID_Almacen IS NULL OR i.ID_Almacen = @ID_Almacen)
       AND (@ID_Categoria IS NULL OR p.ID_Categoria = @ID_Categoria)
       AND (@Buscar IS NULL OR p.CodigoProducto LIKE CONCAT(N'%', @Buscar, N'%') OR p.NombreProducto LIKE CONCAT(N'%', @Buscar, N'%'))
       AND (@SoloConStock = 0 OR i.StockActual > 0)
       AND (@SoloBajo = 0 OR (i.StockMinimo > 0 AND i.StockActual <= i.StockMinimo))
     ORDER BY a.Nombre, p.NombreProducto;
END
GO

-- Kardex de un producto (orden cronológico) o registro general de entradas y salidas
CREATE OR ALTER PROCEDURE sp_Inventario_Movimientos
    @ID_Producto    INT = NULL,
    @ID_Almacen     INT = NULL,
    @TipoMovimiento NVARCHAR(10) = NULL,
    @Desde          DATE = NULL,
    @Hasta          DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT h.ID_Movimiento, h.FechaRegistro, h.TipoMovimiento, h.Concepto, h.Documento,
           h.ID_Almacen, a.Nombre AS Almacen, h.ID_Producto, p.CodigoProducto, p.NombreProducto,
           CASE WHEN h.TipoMovimiento = N'ENTRADA' THEN h.Cantidad ELSE 0 END AS Entrada,
           CASE WHEN h.TipoMovimiento = N'SALIDA' THEN h.Cantidad ELSE 0 END AS Salida,
           h.CostoUnitario, h.SaldoAnterior_Almacen, h.SaldoActual_Almacen, h.SaldoActual_Producto,
           u.NombreCompleto AS Usuario
      FROM HistorialMovimientos h
      JOIN Almacen a ON a.ID_Almacen = h.ID_Almacen
      JOIN Producto p ON p.ID_Producto = h.ID_Producto
      JOIN Usuario u ON u.ID_Usuario = h.ID_Usuario
     WHERE (@ID_Producto IS NULL OR h.ID_Producto = @ID_Producto)
       AND (@ID_Almacen IS NULL OR h.ID_Almacen = @ID_Almacen)
       AND (@TipoMovimiento IS NULL OR h.TipoMovimiento = UPPER(@TipoMovimiento))
       AND (@Desde IS NULL OR h.FechaRegistro >= @Desde)
       AND (@Hasta IS NULL OR h.FechaRegistro < DATEADD(DAY, 1, @Hasta))
     ORDER BY CASE WHEN @ID_Producto IS NULL THEN -h.ID_Movimiento ELSE h.ID_Movimiento END;
END
GO

-- Total de productos, unidades y valor del inventario por almacén
CREATE OR ALTER PROCEDURE sp_Inventario_Resumen
AS
BEGIN
    SET NOCOUNT ON;
    SELECT a.ID_Almacen, a.Nombre AS Almacen,
           COUNT(i.ID_Producto) AS Productos,
           ISNULL(SUM(i.StockActual), 0) AS Unidades,
           CAST(ISNULL(SUM(i.StockActual * p.Costo), 0) AS DECIMAL(18,2)) AS ValorInventario
      FROM Almacen a
      LEFT JOIN InventarioAlmacen i ON i.ID_Almacen = a.ID_Almacen AND i.StockActual > 0
      LEFT JOIN Producto p ON p.ID_Producto = i.ID_Producto
     WHERE a.Estado = 1
     GROUP BY a.ID_Almacen, a.Nombre
     ORDER BY a.Nombre;
END
GO

-- Define el stock mínimo y máximo de un producto en un almacén
CREATE OR ALTER PROCEDURE sp_Inventario_ActualizarLimites
    @ID_Almacen  INT,
    @ID_Producto INT,
    @StockMinimo INT,
    @StockMaximo INT
AS
BEGIN
    SET NOCOUNT ON;

    IF @StockMinimo < 0 OR @StockMaximo < 0
        THROW 50001, N'Los límites no pueden ser negativos', 1;

    IF @StockMaximo > 0 AND @StockMinimo > @StockMaximo
        THROW 50001, N'El stock mínimo no puede ser mayor que el máximo', 1;

    IF NOT EXISTS (SELECT 1 FROM Almacen WHERE ID_Almacen = @ID_Almacen)
       OR NOT EXISTS (SELECT 1 FROM Producto WHERE ID_Producto = @ID_Producto)
        THROW 50404, N'El almacén o el producto no existe', 1;

    IF EXISTS (SELECT 1 FROM InventarioAlmacen WHERE ID_Almacen = @ID_Almacen AND ID_Producto = @ID_Producto)
        UPDATE InventarioAlmacen
           SET StockMinimo = @StockMinimo, StockMaximo = @StockMaximo
         WHERE ID_Almacen = @ID_Almacen AND ID_Producto = @ID_Producto;
    ELSE
        INSERT INTO InventarioAlmacen (ID_Almacen, ID_Producto, StockActual, StockMinimo, StockMaximo)
        VALUES (@ID_Almacen, @ID_Producto, 0, @StockMinimo, @StockMaximo);
END
GO

-- ---------- DASHBOARD ----------

-- Indicadores principales (de un almacén o de todos si @ID_Almacen es NULL)
CREATE OR ALTER PROCEDURE sp_Dashboard_Indicadores
    @ID_Almacen INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Hoy DATE = CAST(GETDATE() AS DATE);
    DECLARE @InicioMes DATE = DATEFROMPARTS(YEAR(@Hoy), MONTH(@Hoy), 1);

    -- Ventas netas = ventas + notas de débito - notas de crédito
    SELECT
        CAST(ISNULL((SELECT SUM(CASE tt.Codigo WHEN N'NC_VENTA' THEN -t.Total ELSE t.Total END)
                       FROM Transaccion t JOIN TipoTransaccion tt ON tt.ID_TipoTransaccion = t.ID_TipoTransaccion
                      WHERE tt.Codigo IN (N'VENTA', N'ND_VENTA', N'NC_VENTA') AND t.Estado = N'REGISTRADA'
                        AND t.Fecha >= @Hoy AND (@ID_Almacen IS NULL OR t.ID_Almacen = @ID_Almacen)), 0) AS DECIMAL(18,2)) AS VentasHoy,
        CAST(ISNULL((SELECT SUM(CASE tt.Codigo WHEN N'NC_VENTA' THEN -t.Total ELSE t.Total END)
                       FROM Transaccion t JOIN TipoTransaccion tt ON tt.ID_TipoTransaccion = t.ID_TipoTransaccion
                      WHERE tt.Codigo IN (N'VENTA', N'ND_VENTA', N'NC_VENTA') AND t.Estado = N'REGISTRADA'
                        AND t.Fecha >= @InicioMes AND (@ID_Almacen IS NULL OR t.ID_Almacen = @ID_Almacen)), 0) AS DECIMAL(18,2)) AS VentasMes,
        (SELECT COUNT(*) FROM Transaccion t JOIN TipoTransaccion tt ON tt.ID_TipoTransaccion = t.ID_TipoTransaccion
          WHERE tt.Codigo = N'VENTA' AND t.Estado = N'REGISTRADA' AND t.Fecha >= @InicioMes
            AND (@ID_Almacen IS NULL OR t.ID_Almacen = @ID_Almacen)) AS CantidadVentasMes,
        CAST(ISNULL((SELECT SUM(CASE tt.Codigo WHEN N'NC_COMPRA' THEN -t.Total ELSE t.Total END)
                       FROM Transaccion t JOIN TipoTransaccion tt ON tt.ID_TipoTransaccion = t.ID_TipoTransaccion
                      WHERE tt.Codigo IN (N'COMPRA', N'ND_COMPRA', N'NC_COMPRA') AND t.Estado = N'REGISTRADA'
                        AND t.Fecha >= @InicioMes AND (@ID_Almacen IS NULL OR t.ID_Almacen = @ID_Almacen)), 0) AS DECIMAL(18,2)) AS ComprasMes,
        (SELECT COUNT(*) FROM Producto WHERE Estado = 1) AS ProductosActivos,
        (SELECT COUNT(*) FROM InventarioAlmacen
          WHERE StockMinimo > 0 AND StockActual <= StockMinimo
            AND (@ID_Almacen IS NULL OR ID_Almacen = @ID_Almacen)) AS ProductosStockBajo,
        CAST(ISNULL((SELECT SUM(i.StockActual * p.Costo) FROM InventarioAlmacen i JOIN Producto p ON p.ID_Producto = i.ID_Producto
                      WHERE @ID_Almacen IS NULL OR i.ID_Almacen = @ID_Almacen), 0) AS DECIMAL(18,2)) AS ValorInventario,
        (SELECT COUNT(*) FROM Transferencia
          WHERE Estado = N'ENVIADA' AND (@ID_Almacen IS NULL OR ID_AlmacenDestino = @ID_Almacen)) AS TransferenciasPorRecibir,
        (SELECT COUNT(*) FROM OrdenCompra
          WHERE Estado IN (N'PENDIENTE', N'PARCIAL') AND (@ID_Almacen IS NULL OR ID_Almacen = @ID_Almacen)) AS OrdenesPendientes;
END
GO

-- Ventas netas por día de los últimos @Dias días (incluye los días sin ventas)
CREATE OR ALTER PROCEDURE sp_Dashboard_VentasPorDia
    @ID_Almacen INT = NULL,
    @Dias       INT = 7
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Hoy DATE = CAST(GETDATE() AS DATE);

    WITH Dias AS (
        SELECT DATEADD(DAY, -(@Dias - 1), @Hoy) AS Fecha
        UNION ALL
        SELECT DATEADD(DAY, 1, Fecha) FROM Dias WHERE Fecha < @Hoy
    )
    SELECT CAST(d.Fecha AS DATETIME2) AS Fecha,
           CAST(ISNULL(SUM(v.Monto), 0) AS DECIMAL(18,2)) AS Total
      FROM Dias d
      LEFT JOIN (SELECT CAST(t.Fecha AS DATE) AS Dia,
                        CASE tt.Codigo WHEN N'NC_VENTA' THEN -t.Total ELSE t.Total END AS Monto
                   FROM Transaccion t
                   JOIN TipoTransaccion tt ON tt.ID_TipoTransaccion = t.ID_TipoTransaccion
                  WHERE tt.Codigo IN (N'VENTA', N'ND_VENTA', N'NC_VENTA') AND t.Estado = N'REGISTRADA'
                    AND (@ID_Almacen IS NULL OR t.ID_Almacen = @ID_Almacen)) v ON v.Dia = d.Fecha
     GROUP BY d.Fecha
     ORDER BY d.Fecha
    OPTION (MAXRECURSION 400);
END
GO

-- Los 5 productos más vendidos del mes
CREATE OR ALTER PROCEDURE sp_Dashboard_TopProductos
    @ID_Almacen INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @InicioMes DATE = DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1);

    SELECT TOP 5 p.ID_Producto, p.CodigoProducto, p.NombreProducto,
           SUM(d.Cantidad) AS Cantidad, CAST(SUM(d.Total) AS DECIMAL(18,2)) AS Total
      FROM TransaccionDetalle d
      JOIN Transaccion t ON t.ID_Transaccion = d.ID_Transaccion
      JOIN TipoTransaccion tt ON tt.ID_TipoTransaccion = t.ID_TipoTransaccion
      JOIN Producto p ON p.ID_Producto = d.ID_Producto
     WHERE tt.Codigo = N'VENTA' AND t.Estado = N'REGISTRADA' AND t.Fecha >= @InicioMes
       AND (@ID_Almacen IS NULL OR t.ID_Almacen = @ID_Almacen)
     GROUP BY p.ID_Producto, p.CodigoProducto, p.NombreProducto
     ORDER BY SUM(d.Cantidad) DESC;
END
GO
