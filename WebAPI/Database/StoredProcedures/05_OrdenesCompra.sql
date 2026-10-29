/* =====================================================================
   05 - ÓRDENES DE COMPRA
   La orden es solo el pedido al proveedor: NO mueve inventario.
   La mercadería entra cuando se registra una COMPRA que hace referencia a la orden.
   ===================================================================== */

CREATE OR ALTER PROCEDURE sp_OrdenCompra_Listar
    @Estado       NVARCHAR(20) = NULL,
    @ID_Proveedor INT = NULL,
    @ID_Almacen   INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT o.ID_OrdenCompra, o.NumeroOrden, o.ID_Proveedor, d.Nombre AS Proveedor, o.ID_Almacen, a.Nombre AS Almacen,
           o.ID_Usuario, u.NombreCompleto AS Usuario, o.FechaOrden, o.FechaEsperada, o.Estado,
           o.Subtotal, o.Impuesto, o.Total, o.Observaciones
      FROM OrdenCompra o
      JOIN Proveedor p ON p.ID_Proveedor = o.ID_Proveedor
      JOIN DirectorioContacto d ON d.ID_Contacto = p.ID_Contacto
      JOIN Almacen a ON a.ID_Almacen = o.ID_Almacen
      JOIN Usuario u ON u.ID_Usuario = o.ID_Usuario
     WHERE (@Estado IS NULL OR o.Estado = UPPER(@Estado))
       AND (@ID_Proveedor IS NULL OR o.ID_Proveedor = @ID_Proveedor)
       AND (@ID_Almacen IS NULL OR o.ID_Almacen = @ID_Almacen)
     ORDER BY o.FechaOrden DESC;
END
GO

CREATE OR ALTER PROCEDURE sp_OrdenCompra_Obtener
    @ID_OrdenCompra INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT o.ID_OrdenCompra, o.NumeroOrden, o.ID_Proveedor, d.Nombre AS Proveedor, o.ID_Almacen, a.Nombre AS Almacen,
           o.ID_Usuario, u.NombreCompleto AS Usuario, o.FechaOrden, o.FechaEsperada, o.Estado,
           o.Subtotal, o.Impuesto, o.Total, o.Observaciones
      FROM OrdenCompra o
      JOIN Proveedor p ON p.ID_Proveedor = o.ID_Proveedor
      JOIN DirectorioContacto d ON d.ID_Contacto = p.ID_Contacto
      JOIN Almacen a ON a.ID_Almacen = o.ID_Almacen
      JOIN Usuario u ON u.ID_Usuario = o.ID_Usuario
     WHERE o.ID_OrdenCompra = @ID_OrdenCompra;
END
GO

CREATE OR ALTER PROCEDURE sp_OrdenCompra_Detalle
    @ID_OrdenCompra INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT d.ID_DetalleOC, d.ID_Producto, p.CodigoProducto, p.NombreProducto,
           d.CantidadPedida, d.CantidadRecibida, d.CantidadPedida - d.CantidadRecibida AS CantidadPendiente,
           d.CostoUnitario, d.Descuento, d.Total
      FROM OrdenCompraDetalle d
      JOIN Producto p ON p.ID_Producto = d.ID_Producto
     WHERE d.ID_OrdenCompra = @ID_OrdenCompra
     ORDER BY d.ID_DetalleOC;
END
GO

/* Inserta o actualiza una orden (@ID_OrdenCompra = 0 o NULL para crear una nueva).
   @Detalles es JSON: [{"ID_Producto":1,"Cantidad":20,"CostoUnitario":6.50,"Descuento":0}]
   Devuelve el ID de la orden */
CREATE OR ALTER PROCEDURE sp_OrdenCompra_Guardar
    @ID_OrdenCompra INT = NULL,
    @ID_Proveedor   INT,
    @ID_Almacen     INT,
    @ID_Usuario     INT,
    @FechaEsperada  DATE = NULL,
    @Observaciones  NVARCHAR(255) = NULL,
    @Detalles       NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    DECLARE @Msg NVARCHAR(400), @Iva DECIMAL(9,4), @Subtotal DECIMAL(18,2), @Numero NVARCHAR(30), @EstadoActual NVARCHAR(20);

    IF ISNULL(@ID_OrdenCompra, 0) <> 0
    BEGIN
        SELECT @EstadoActual = Estado FROM OrdenCompra WHERE ID_OrdenCompra = @ID_OrdenCompra;

        IF @EstadoActual IS NULL
            THROW 50404, N'La orden de compra no existe', 1;

        IF @EstadoActual <> N'PENDIENTE'
        BEGIN
            SET @Msg = CONCAT(N'No se puede modificar una orden en estado ', @EstadoActual);
            THROW 50001, @Msg, 1;
        END
    END

    IF NOT EXISTS (SELECT 1 FROM Proveedor WHERE ID_Proveedor = @ID_Proveedor AND Estado = 1)
        THROW 50001, N'El proveedor no existe o está inactivo', 1;

    EXEC sp_Usuario_ValidarAlmacen @ID_Usuario, @ID_Almacen;

    IF ISJSON(@Detalles) = 0
        THROW 50001, N'El detalle no tiene un formato válido', 1;

    -- Se lee el detalle JSON en una tabla variable
    DECLARE @Lineas TABLE (ID_Producto INT, Cantidad INT, CostoUnitario DECIMAL(18,2), Descuento DECIMAL(18,2));

    INSERT INTO @Lineas (ID_Producto, Cantidad, CostoUnitario, Descuento)
    SELECT ID_Producto, Cantidad, ISNULL(CostoUnitario, 0), ISNULL(Descuento, 0)
      FROM OPENJSON(@Detalles)
      WITH (ID_Producto INT '$.ID_Producto', Cantidad INT '$.Cantidad',
            CostoUnitario DECIMAL(18,2) '$.CostoUnitario', Descuento DECIMAL(18,2) '$.Descuento');

    IF NOT EXISTS (SELECT 1 FROM @Lineas)
        THROW 50001, N'La orden debe tener al menos un producto', 1;

    IF EXISTS (SELECT ID_Producto FROM @Lineas GROUP BY ID_Producto HAVING COUNT(*) > 1)
        THROW 50001, N'Un producto no se puede repetir en la misma orden', 1;

    IF EXISTS (SELECT 1 FROM @Lineas WHERE Cantidad IS NULL OR Cantidad <= 0 OR CostoUnitario < 0 OR Descuento < 0)
        THROW 50001, N'Las cantidades deben ser mayores a cero y los montos no pueden ser negativos', 1;

    SELECT TOP 1 @Msg = CONCAT(N'El producto ', l.ID_Producto, N' no existe o está inactivo')
      FROM @Lineas l
      LEFT JOIN Producto p ON p.ID_Producto = l.ID_Producto AND p.Estado = 1
     WHERE p.ID_Producto IS NULL;
    IF @Msg IS NOT NULL THROW 50001, @Msg, 1;

    SELECT TOP 1 @Msg = CONCAT(N'El descuento de ''', p.NombreProducto, N''' es mayor que el importe')
      FROM @Lineas l JOIN Producto p ON p.ID_Producto = l.ID_Producto
     WHERE l.Descuento > l.Cantidad * l.CostoUnitario;
    IF @Msg IS NOT NULL THROW 50001, @Msg, 1;

    -- Totales: subtotal ya con descuento, más IVA
    EXEC sp_Parametro_ObtenerIva @ID_Almacen, @Iva OUTPUT;
    SELECT @Subtotal = SUM(ROUND(Cantidad * CostoUnitario - Descuento, 2)) FROM @Lineas;

    BEGIN TRY
        BEGIN TRANSACTION;

        IF ISNULL(@ID_OrdenCompra, 0) = 0
        BEGIN
            EXEC sp_Correlativo_Siguiente N'ORDEN_COMPRA', @Numero OUTPUT;

            INSERT INTO OrdenCompra (NumeroOrden, ID_Proveedor, ID_Almacen, ID_Usuario, FechaOrden, FechaEsperada, Estado,
                                     Subtotal, Impuesto, Total, Observaciones)
            VALUES (@Numero, @ID_Proveedor, @ID_Almacen, @ID_Usuario, GETDATE(), @FechaEsperada, N'PENDIENTE',
                    @Subtotal, ROUND(@Subtotal * @Iva / 100, 2), @Subtotal + ROUND(@Subtotal * @Iva / 100, 2), @Observaciones);

            SET @ID_OrdenCompra = SCOPE_IDENTITY();
        END
        ELSE
        BEGIN
            UPDATE OrdenCompra
               SET ID_Proveedor = @ID_Proveedor, ID_Almacen = @ID_Almacen, FechaEsperada = @FechaEsperada,
                   Observaciones = @Observaciones, Subtotal = @Subtotal,
                   Impuesto = ROUND(@Subtotal * @Iva / 100, 2), Total = @Subtotal + ROUND(@Subtotal * @Iva / 100, 2),
                   FechaModificacion = GETDATE()
             WHERE ID_OrdenCompra = @ID_OrdenCompra;

            DELETE FROM OrdenCompraDetalle WHERE ID_OrdenCompra = @ID_OrdenCompra;
        END

        INSERT INTO OrdenCompraDetalle (ID_OrdenCompra, ID_Producto, CantidadPedida, CantidadRecibida, CostoUnitario, Descuento, Total)
        SELECT @ID_OrdenCompra, ID_Producto, Cantidad, 0, CostoUnitario, Descuento, ROUND(Cantidad * CostoUnitario - Descuento, 2)
          FROM @Lineas;

        COMMIT;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK;
        THROW;
    END CATCH

    SELECT @ID_OrdenCompra AS Value;
END
GO

-- Si estaba PARCIAL, lo recibido se queda en inventario y se cancela lo pendiente
CREATE OR ALTER PROCEDURE sp_OrdenCompra_Cancelar
    @ID_OrdenCompra INT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Estado NVARCHAR(20), @Msg NVARCHAR(200);

    SELECT @Estado = Estado FROM OrdenCompra WHERE ID_OrdenCompra = @ID_OrdenCompra;

    IF @Estado IS NULL
        THROW 50404, N'La orden de compra no existe', 1;

    IF @Estado NOT IN (N'PENDIENTE', N'PARCIAL')
    BEGIN
        SET @Msg = CONCAT(N'No se puede cancelar una orden en estado ', @Estado);
        THROW 50001, @Msg, 1;
    END

    UPDATE OrdenCompra SET Estado = N'CANCELADA', FechaModificacion = GETDATE() WHERE ID_OrdenCompra = @ID_OrdenCompra;
END
GO
