/* =====================================================================
   09 - PRODUCTOS POR ALMACÉN (tabla InventarioAlmacen)
   Define qué productos maneja cada almacén, cuánto hay y sus límites de stock.
   La existencia NUNCA se escribe directo: cuando se indica una cantidad, el SP registra un
   Ajuste de entrada / salida con sp_Transaccion_Registrar, así queda el documento (AJE/AJS)
   y el movimiento en el kardex.
   La consulta se hace con sp_Inventario_Existencias (08_Reportes.sql).
   ===================================================================== */

-- Busca el motivo de ajuste por su descripción; si no existe usa el primero activo
CREATE OR ALTER PROCEDURE sp_MotivoAjuste_Buscar
    @Descripcion NVARCHAR(100),
    @ID_Motivo   INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET @ID_Motivo = NULL;

    SELECT @ID_Motivo = ID_Motivo FROM MotivoAjuste WHERE Descripcion = @Descripcion AND Estado = 1;

    IF @ID_Motivo IS NULL
        SELECT TOP 1 @ID_Motivo = ID_Motivo FROM MotivoAjuste WHERE Estado = 1 ORDER BY ID_Motivo;

    IF @ID_Motivo IS NULL
        THROW 50001, N'No hay motivos de ajuste activos. Cree uno en Catálogos > Motivos de ajuste', 1;
END
GO

/* Asigna uno o varios productos a un almacén con su existencia actual y límites.
   @Productos es JSON:
   [{"ID_Producto":1,"Cantidad":25,"CostoUnitario":5.50,"StockMinimo":10,"StockMaximo":100}, ...]
   - Cantidad > 0 registra un AJUSTE_ENTRADA con motivo "Inventario inicial"
   - CostoUnitario vacío = costo actual del producto
   Devuelve cuántos productos se asignaron */
CREATE OR ALTER PROCEDURE sp_InventarioAlmacen_Asignar
    @ID_Almacen INT,
    @ID_Usuario INT,
    @Productos  NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    DECLARE @Msg NVARCHAR(300), @Total INT, @ID_Motivo INT, @Detalles NVARCHAR(MAX);

    EXEC sp_Usuario_ValidarAlmacen @ID_Usuario, @ID_Almacen;

    IF ISJSON(@Productos) = 0
        THROW 50001, N'La lista de productos no tiene un formato válido', 1;

    DECLARE @Lista TABLE (
        ID_Producto INT PRIMARY KEY, Cantidad INT, CostoUnitario DECIMAL(18,2) NULL, StockMinimo INT, StockMaximo INT);

    INSERT INTO @Lista (ID_Producto, Cantidad, CostoUnitario, StockMinimo, StockMaximo)
    SELECT ID_Producto, ISNULL(MAX(Cantidad), 0), MAX(CostoUnitario), ISNULL(MAX(StockMinimo), 0), ISNULL(MAX(StockMaximo), 0)
      FROM OPENJSON(@Productos)
      WITH (ID_Producto INT '$.ID_Producto', Cantidad INT '$.Cantidad', CostoUnitario DECIMAL(18,2) '$.CostoUnitario',
            StockMinimo INT '$.StockMinimo', StockMaximo INT '$.StockMaximo')
     WHERE ID_Producto IS NOT NULL
     GROUP BY ID_Producto;

    SET @Total = @@ROWCOUNT;

    IF @Total = 0
        THROW 50001, N'Seleccione al menos un producto', 1;

    IF EXISTS (SELECT 1 FROM @Lista WHERE Cantidad < 0 OR StockMinimo < 0 OR StockMaximo < 0 OR CostoUnitario < 0)
        THROW 50001, N'Las cantidades, costos y límites no pueden ser negativos', 1;

    SELECT TOP 1 @Msg = CONCAT(N'El producto ', l.ID_Producto, N' no existe o está inactivo')
      FROM @Lista l
      LEFT JOIN Producto p ON p.ID_Producto = l.ID_Producto AND p.Estado = 1
     WHERE p.ID_Producto IS NULL;
    IF @Msg IS NOT NULL THROW 50001, @Msg, 1;

    SELECT TOP 1 @Msg = CONCAT(N'''', p.NombreProducto, N''': el stock mínimo no puede ser mayor que el máximo')
      FROM @Lista l JOIN Producto p ON p.ID_Producto = l.ID_Producto
     WHERE l.StockMaximo > 0 AND l.StockMinimo > l.StockMaximo;
    IF @Msg IS NOT NULL THROW 50001, @Msg, 1;

    SELECT TOP 1 @Msg = CONCAT(N'''', p.NombreProducto, N''' ya está asignado a este almacén')
      FROM @Lista l
      JOIN Producto p ON p.ID_Producto = l.ID_Producto
      JOIN InventarioAlmacen i ON i.ID_Almacen = @ID_Almacen AND i.ID_Producto = l.ID_Producto;
    IF @Msg IS NOT NULL THROW 50001, @Msg, 1;

    BEGIN TRY
        BEGIN TRANSACTION;

        -- 1. Se crea la fila de cada producto con existencia 0 y sus límites
        INSERT INTO InventarioAlmacen (ID_Almacen, ID_Producto, StockActual, StockMinimo, StockMaximo)
        SELECT @ID_Almacen, ID_Producto, 0, StockMinimo, StockMaximo FROM @Lista;

        -- 2. La existencia inicial entra con un ajuste de entrada (genera documento AJE y kardex)
        IF EXISTS (SELECT 1 FROM @Lista WHERE Cantidad > 0)
        BEGIN
            EXEC sp_MotivoAjuste_Buscar N'Inventario inicial', @ID_Motivo OUTPUT;

            SET @Detalles = (SELECT ID_Producto, Cantidad, CostoUnitario AS PrecioUnitario, 0 AS Descuento, @ID_Motivo AS ID_Motivo
                               FROM @Lista WHERE Cantidad > 0
                                FOR JSON PATH, INCLUDE_NULL_VALUES);

            EXEC sp_Transaccion_Registrar
                 @CodigoTipoTransaccion = N'AJUSTE_ENTRADA', @ID_Usuario = @ID_Usuario, @ID_Almacen = @ID_Almacen,
                 @Observaciones = N'Existencia inicial al asignar productos al almacén',
                 @Detalles = @Detalles, @SinResultado = 1;
        END

        COMMIT;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK;
        THROW;
    END CATCH

    SELECT @Total AS Value;
END
GO

/* Cambia los límites y, si se indica @CantidadActual (conteo físico), ajusta la existencia:
   registra un AJUSTE_ENTRADA o AJUSTE_SALIDA por la diferencia con motivo "Diferencia en conteo físico" */
CREATE OR ALTER PROCEDURE sp_InventarioAlmacen_Actualizar
    @ID_Almacen     INT,
    @ID_Producto    INT,
    @ID_Usuario     INT,
    @StockMinimo    INT,
    @StockMaximo    INT,
    @CantidadActual INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    DECLARE @StockActual INT, @Diferencia INT, @ID_Motivo INT, @Detalles NVARCHAR(MAX), @Tipo NVARCHAR(20);

    SELECT @StockActual = StockActual FROM InventarioAlmacen WHERE ID_Almacen = @ID_Almacen AND ID_Producto = @ID_Producto;

    IF @StockActual IS NULL
        THROW 50404, N'El producto no está asignado a ese almacén', 1;

    IF @StockMinimo < 0 OR @StockMaximo < 0 OR @CantidadActual < 0
        THROW 50001, N'Las cantidades y límites no pueden ser negativos', 1;

    IF @StockMaximo > 0 AND @StockMinimo > @StockMaximo
        THROW 50001, N'El stock mínimo no puede ser mayor que el máximo', 1;

    SET @Diferencia = ISNULL(@CantidadActual, @StockActual) - @StockActual;

    IF @Diferencia <> 0
        EXEC sp_Usuario_ValidarAlmacen @ID_Usuario, @ID_Almacen;

    BEGIN TRY
        BEGIN TRANSACTION;

        UPDATE InventarioAlmacen
           SET StockMinimo = @StockMinimo, StockMaximo = @StockMaximo
         WHERE ID_Almacen = @ID_Almacen AND ID_Producto = @ID_Producto;

        IF @Diferencia <> 0
        BEGIN
            EXEC sp_MotivoAjuste_Buscar N'Diferencia en conteo físico', @ID_Motivo OUTPUT;

            SET @Tipo = CASE WHEN @Diferencia > 0 THEN N'AJUSTE_ENTRADA' ELSE N'AJUSTE_SALIDA' END;
            SET @Detalles = (SELECT @ID_Producto AS ID_Producto, ABS(@Diferencia) AS Cantidad, 0 AS Descuento, @ID_Motivo AS ID_Motivo
                                FOR JSON PATH);

            EXEC sp_Transaccion_Registrar
                 @CodigoTipoTransaccion = @Tipo, @ID_Usuario = @ID_Usuario, @ID_Almacen = @ID_Almacen,
                 @Observaciones = N'Ajuste por conteo físico desde Productos por almacén',
                 @Detalles = @Detalles, @SinResultado = 1;
        END

        COMMIT;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK;
        THROW;
    END CATCH
END
GO

-- Quita un producto de un almacén. Solo se permite si no tiene existencia
CREATE OR ALTER PROCEDURE sp_InventarioAlmacen_Quitar
    @ID_Almacen  INT,
    @ID_Producto INT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Stock INT, @Msg NVARCHAR(300);

    SELECT @Stock = StockActual FROM InventarioAlmacen WHERE ID_Almacen = @ID_Almacen AND ID_Producto = @ID_Producto;

    IF @Stock IS NULL
        THROW 50404, N'El producto no está asignado a ese almacén', 1;

    IF @Stock <> 0
    BEGIN
        SET @Msg = CONCAT(N'No se puede quitar: el producto todavía tiene ', @Stock,
                          N' unidades en el almacén. Primero sáquelas con una venta, transferencia o ajuste.');
        THROW 50001, @Msg, 1;
    END

    IF EXISTS (SELECT 1 FROM Transferencia t
                 JOIN TransferenciaDetalle d ON d.ID_Transferencia = t.ID_Transferencia
                WHERE t.Estado = N'ENVIADA' AND d.ID_Producto = @ID_Producto
                  AND (t.ID_AlmacenDestino = @ID_Almacen OR t.ID_AlmacenOrigen = @ID_Almacen))
        THROW 50001, N'No se puede quitar: hay una transferencia pendiente con ese producto', 1;

    -- El historial (kardex) no se borra: queda guardado en HistorialMovimientos
    DELETE FROM InventarioAlmacen WHERE ID_Almacen = @ID_Almacen AND ID_Producto = @ID_Producto;
END
GO
