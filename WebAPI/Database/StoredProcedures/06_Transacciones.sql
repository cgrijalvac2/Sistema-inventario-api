/* =====================================================================
   06 - TRANSACCIONES: compras, ventas, notas de crédito, notas de débito y ajustes
   Cada línea que mueve inventario llama a sp_Inventario_RegistrarMovimiento (kardex)
   ===================================================================== */

CREATE OR ALTER PROCEDURE sp_Transaccion_Listar
    @Tipo         NVARCHAR(20) = NULL,
    @ID_Almacen   INT = NULL,
    @ID_Cliente   INT = NULL,
    @ID_Proveedor INT = NULL,
    @Desde        DATE = NULL,
    @Hasta        DATE = NULL,
    @Estado       NVARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT t.ID_Transaccion, t.NumeroDocumento, tt.Codigo AS CodigoTipoTransaccion, tt.Nombre AS TipoTransaccion,
           t.Fecha, t.ID_Almacen, a.Nombre AS Almacen, t.ID_Usuario, u.NombreCompleto AS Usuario,
           t.ID_Cliente, dc.Nombre AS Cliente, t.ID_Proveedor, dp.Nombre AS Proveedor,
           t.ID_OrdenCompra, o.NumeroOrden, t.ID_TransaccionOrigen, ori.NumeroDocumento AS DocumentoOrigen,
           t.Referencia, t.Subtotal, t.Descuento, t.Impuesto, t.Total, t.Estado, t.Observaciones, t.FechaAnulacion
      FROM Transaccion t
      JOIN TipoTransaccion tt ON tt.ID_TipoTransaccion = t.ID_TipoTransaccion
      JOIN Almacen a ON a.ID_Almacen = t.ID_Almacen
      JOIN Usuario u ON u.ID_Usuario = t.ID_Usuario
      LEFT JOIN Cliente c ON c.ID_Cliente = t.ID_Cliente
      LEFT JOIN DirectorioContacto dc ON dc.ID_Contacto = c.ID_Contacto
      LEFT JOIN Proveedor p ON p.ID_Proveedor = t.ID_Proveedor
      LEFT JOIN DirectorioContacto dp ON dp.ID_Contacto = p.ID_Contacto
      LEFT JOIN OrdenCompra o ON o.ID_OrdenCompra = t.ID_OrdenCompra
      LEFT JOIN Transaccion ori ON ori.ID_Transaccion = t.ID_TransaccionOrigen
     WHERE (@Tipo IS NULL OR tt.Codigo = UPPER(@Tipo))
       AND (@ID_Almacen IS NULL OR t.ID_Almacen = @ID_Almacen)
       AND (@ID_Cliente IS NULL OR t.ID_Cliente = @ID_Cliente)
       AND (@ID_Proveedor IS NULL OR t.ID_Proveedor = @ID_Proveedor)
       AND (@Desde IS NULL OR t.Fecha >= @Desde)
       AND (@Hasta IS NULL OR t.Fecha < DATEADD(DAY, 1, @Hasta))
       AND (@Estado IS NULL OR t.Estado = UPPER(@Estado))
     ORDER BY t.Fecha DESC, t.ID_Transaccion DESC;
END
GO

CREATE OR ALTER PROCEDURE sp_Transaccion_Obtener
    @ID_Transaccion INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT t.ID_Transaccion, t.NumeroDocumento, tt.Codigo AS CodigoTipoTransaccion, tt.Nombre AS TipoTransaccion,
           t.Fecha, t.ID_Almacen, a.Nombre AS Almacen, t.ID_Usuario, u.NombreCompleto AS Usuario,
           t.ID_Cliente, dc.Nombre AS Cliente, t.ID_Proveedor, dp.Nombre AS Proveedor,
           t.ID_OrdenCompra, o.NumeroOrden, t.ID_TransaccionOrigen, ori.NumeroDocumento AS DocumentoOrigen,
           t.Referencia, t.Subtotal, t.Descuento, t.Impuesto, t.Total, t.Estado, t.Observaciones, t.FechaAnulacion
      FROM Transaccion t
      JOIN TipoTransaccion tt ON tt.ID_TipoTransaccion = t.ID_TipoTransaccion
      JOIN Almacen a ON a.ID_Almacen = t.ID_Almacen
      JOIN Usuario u ON u.ID_Usuario = t.ID_Usuario
      LEFT JOIN Cliente c ON c.ID_Cliente = t.ID_Cliente
      LEFT JOIN DirectorioContacto dc ON dc.ID_Contacto = c.ID_Contacto
      LEFT JOIN Proveedor p ON p.ID_Proveedor = t.ID_Proveedor
      LEFT JOIN DirectorioContacto dp ON dp.ID_Contacto = p.ID_Contacto
      LEFT JOIN OrdenCompra o ON o.ID_OrdenCompra = t.ID_OrdenCompra
      LEFT JOIN Transaccion ori ON ori.ID_Transaccion = t.ID_TransaccionOrigen
     WHERE t.ID_Transaccion = @ID_Transaccion;
END
GO

CREATE OR ALTER PROCEDURE sp_Transaccion_Detalle
    @ID_Transaccion INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT d.ID_Detalle, d.ID_Producto, p.CodigoProducto, p.NombreProducto, d.ID_Motivo, m.Descripcion AS Motivo,
           d.Cantidad, d.PrecioUnitario, d.CostoUnitario, d.Descuento, d.Total
      FROM TransaccionDetalle d
      JOIN Producto p ON p.ID_Producto = d.ID_Producto
      LEFT JOIN MotivoAjuste m ON m.ID_Motivo = d.ID_Motivo
     WHERE d.ID_Transaccion = @ID_Transaccion
     ORDER BY d.ID_Detalle;
END
GO

/* Registra cualquier tipo de transacción.
   @Detalles es JSON: [{"ID_Producto":1,"Cantidad":5,"PrecioUnitario":10.00,"Descuento":0,"ID_Motivo":null}]
   Si PrecioUnitario viene vacío se sugiere: el de la venta/compra de origen, el de la orden de compra,
   el precio de venta (documentos a clientes) o el costo del producto (compras y ajustes).
   Devuelve el ID de la transacción */
CREATE OR ALTER PROCEDURE sp_Transaccion_Registrar
    @CodigoTipoTransaccion NVARCHAR(20),
    @ID_Usuario            INT,
    @ID_Almacen            INT,
    @ID_Cliente            INT = NULL,
    @ID_Proveedor          INT = NULL,
    @ID_OrdenCompra        INT = NULL,
    @ID_TransaccionOrigen  INT = NULL,
    @Referencia            NVARCHAR(100) = NULL,
    @Observaciones         NVARCHAR(255) = NULL,
    @Detalles              NVARCHAR(MAX),
    -- 1 cuando lo llama otro SP (ej: sp_InventarioAlmacen_Asignar): no devuelve el SELECT final
    @SinResultado          BIT = 0
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @Msg NVARCHAR(400);
    DECLARE @ID_TipoTransaccion INT, @CodigoTipo NVARCHAR(20), @Factor INT, @TipoTercero NVARCHAR(10),
            @RequiereOrigen BIT, @RequiereMotivo BIT;

    -- ===== 1. Tipo de transacción =====
    SELECT @ID_TipoTransaccion = ID_TipoTransaccion, @CodigoTipo = Codigo, @Factor = Factor_Kardex,
           @TipoTercero = TipoTercero, @RequiereOrigen = RequiereDocumentoOrigen, @RequiereMotivo = RequiereMotivo
      FROM TipoTransaccion
     WHERE Codigo = UPPER(@CodigoTipoTransaccion) AND Estado = 1;

    IF @ID_TipoTransaccion IS NULL
        THROW 50001, N'El tipo de transacción no existe o está inactivo', 1;

    EXEC sp_Usuario_ValidarAlmacen @ID_Usuario, @ID_Almacen;

    -- ===== 2. Cliente o proveedor según el tipo =====
    IF @TipoTercero = N'CLIENTE'
    BEGIN
        IF NOT EXISTS (SELECT 1 FROM Cliente WHERE ID_Cliente = @ID_Cliente AND Estado = 1)
            THROW 50001, N'Debe indicar un cliente activo', 1;
        SET @ID_Proveedor = NULL;
    END
    ELSE IF @TipoTercero = N'PROVEEDOR'
    BEGIN
        IF NOT EXISTS (SELECT 1 FROM Proveedor WHERE ID_Proveedor = @ID_Proveedor AND Estado = 1)
            THROW 50001, N'Debe indicar un proveedor activo', 1;
        SET @ID_Cliente = NULL;
    END
    ELSE
        SELECT @ID_Cliente = NULL, @ID_Proveedor = NULL;

    -- ===== 3. Documento de origen (notas de crédito y débito) =====
    IF @RequiereOrigen = 1
    BEGIN
        DECLARE @OrigenCodigo NVARCHAR(20), @OrigenEstado NVARCHAR(20), @OrigenCliente INT, @OrigenProveedor INT,
                @OrigenAlmacen INT, @CodigoEsperado NVARCHAR(20);

        IF @ID_TransaccionOrigen IS NULL
            THROW 50001, N'Debe indicar el documento de origen', 1;

        SELECT @OrigenCodigo = tt.Codigo, @OrigenEstado = t.Estado, @OrigenCliente = t.ID_Cliente,
               @OrigenProveedor = t.ID_Proveedor, @OrigenAlmacen = t.ID_Almacen
          FROM Transaccion t
          JOIN TipoTransaccion tt ON tt.ID_TipoTransaccion = t.ID_TipoTransaccion
         WHERE t.ID_Transaccion = @ID_TransaccionOrigen;

        IF @OrigenCodigo IS NULL
            THROW 50001, N'El documento de origen no existe', 1;

        SET @CodigoEsperado = CASE WHEN @TipoTercero = N'CLIENTE' THEN N'VENTA' ELSE N'COMPRA' END;
        IF @OrigenCodigo <> @CodigoEsperado
        BEGIN
            SET @Msg = CONCAT(N'Esta nota debe hacer referencia a una ', @CodigoEsperado);
            THROW 50001, @Msg, 1;
        END

        IF @OrigenEstado <> N'REGISTRADA'
            THROW 50001, N'El documento de origen está anulado', 1;

        IF ISNULL(@OrigenCliente, 0) <> ISNULL(@ID_Cliente, 0) OR ISNULL(@OrigenProveedor, 0) <> ISNULL(@ID_Proveedor, 0)
            THROW 50001, N'El documento de origen pertenece a otro cliente o proveedor', 1;

        IF @OrigenAlmacen <> @ID_Almacen
            THROW 50001, N'El documento de origen es de otro almacén', 1;
    END
    ELSE
        SET @ID_TransaccionOrigen = NULL;

    -- ===== 4. Orden de compra (solo compras) =====
    IF @ID_OrdenCompra IS NOT NULL
    BEGIN
        DECLARE @OCEstado NVARCHAR(20), @OCProveedor INT, @OCAlmacen INT;

        IF @CodigoTipo <> N'COMPRA'
            THROW 50001, N'Solo una COMPRA puede recibir una orden de compra', 1;

        SELECT @OCEstado = Estado, @OCProveedor = ID_Proveedor, @OCAlmacen = ID_Almacen
          FROM OrdenCompra WHERE ID_OrdenCompra = @ID_OrdenCompra;

        IF @OCEstado IS NULL
            THROW 50001, N'La orden de compra no existe', 1;

        IF @OCEstado NOT IN (N'PENDIENTE', N'PARCIAL')
        BEGIN
            SET @Msg = CONCAT(N'La orden de compra está ', @OCEstado);
            THROW 50001, @Msg, 1;
        END

        IF @OCProveedor <> @ID_Proveedor OR @OCAlmacen <> @ID_Almacen
            THROW 50001, N'La orden de compra es de otro proveedor o de otro almacén', 1;
    END

    -- ===== 5. Detalle (JSON -> tabla variable) =====
    IF ISJSON(@Detalles) = 0
        THROW 50001, N'El detalle no tiene un formato válido', 1;

    DECLARE @Lineas TABLE (
        Fila INT IDENTITY(1,1), ID_Producto INT, Cantidad INT, PrecioUnitario DECIMAL(18,2) NULL,
        Descuento DECIMAL(18,2), ID_Motivo INT NULL, CostoUnitario DECIMAL(18,2) NULL);

    INSERT INTO @Lineas (ID_Producto, Cantidad, PrecioUnitario, Descuento, ID_Motivo)
    SELECT ID_Producto, Cantidad, PrecioUnitario, ISNULL(Descuento, 0), ID_Motivo
      FROM OPENJSON(@Detalles)
      WITH (ID_Producto INT '$.ID_Producto', Cantidad INT '$.Cantidad', PrecioUnitario DECIMAL(18,2) '$.PrecioUnitario',
            Descuento DECIMAL(18,2) '$.Descuento', ID_Motivo INT '$.ID_Motivo');

    IF NOT EXISTS (SELECT 1 FROM @Lineas)
        THROW 50001, N'El documento debe tener al menos un producto', 1;

    IF EXISTS (SELECT ID_Producto FROM @Lineas GROUP BY ID_Producto HAVING COUNT(*) > 1)
        THROW 50001, N'Un producto no se puede repetir en el mismo documento', 1;

    IF EXISTS (SELECT 1 FROM @Lineas WHERE Cantidad IS NULL OR Cantidad <= 0)
        THROW 50001, N'La cantidad debe ser mayor a cero', 1;

    IF EXISTS (SELECT 1 FROM @Lineas WHERE PrecioUnitario < 0 OR Descuento < 0)
        THROW 50001, N'Los precios y descuentos no pueden ser negativos', 1;

    SELECT TOP 1 @Msg = CONCAT(N'El producto ', l.ID_Producto, N' no existe o está inactivo')
      FROM @Lineas l
      LEFT JOIN Producto p ON p.ID_Producto = l.ID_Producto AND p.Estado = 1
     WHERE p.ID_Producto IS NULL;
    IF @Msg IS NOT NULL THROW 50001, @Msg, 1;

    -- Ajustes: cada línea necesita un motivo válido
    IF @RequiereMotivo = 1
    BEGIN
        SELECT TOP 1 @Msg = CONCAT(N'La línea de ''', p.NombreProducto, N''' necesita un motivo de ajuste válido')
          FROM @Lineas l
          JOIN Producto p ON p.ID_Producto = l.ID_Producto
          LEFT JOIN MotivoAjuste m ON m.ID_Motivo = l.ID_Motivo AND m.Estado = 1
         WHERE m.ID_Motivo IS NULL;
        IF @Msg IS NOT NULL THROW 50001, @Msg, 1;
    END
    ELSE
        UPDATE @Lineas SET ID_Motivo = NULL;

    -- Notas: el producto debe estar en el documento de origen y no se puede devolver más de lo facturado
    IF @ID_TransaccionOrigen IS NOT NULL
    BEGIN
        SELECT TOP 1 @Msg = CONCAT(N'''', p.NombreProducto, N''' no está en el documento de origen')
          FROM @Lineas l
          JOIN Producto p ON p.ID_Producto = l.ID_Producto
         WHERE NOT EXISTS (SELECT 1 FROM TransaccionDetalle d
                            WHERE d.ID_Transaccion = @ID_TransaccionOrigen AND d.ID_Producto = l.ID_Producto);
        IF @Msg IS NOT NULL THROW 50001, @Msg, 1;

        UPDATE l SET PrecioUnitario = ISNULL(l.PrecioUnitario, d.PrecioUnitario)
          FROM @Lineas l
          JOIN TransaccionDetalle d ON d.ID_Transaccion = @ID_TransaccionOrigen AND d.ID_Producto = l.ID_Producto;

        IF @Factor <> 0
        BEGIN
            SELECT TOP 1 @Msg = CONCAT(N'Solo se pueden devolver ', d.Cantidad - ISNULL(dev.Devuelto, 0),
                                       N' unidades de ''', p.NombreProducto, N'''')
              FROM @Lineas l
              JOIN TransaccionDetalle d ON d.ID_Transaccion = @ID_TransaccionOrigen AND d.ID_Producto = l.ID_Producto
              JOIN Producto p ON p.ID_Producto = l.ID_Producto
             OUTER APPLY (SELECT SUM(dn.Cantidad) AS Devuelto
                            FROM TransaccionDetalle dn
                            JOIN Transaccion tn ON tn.ID_Transaccion = dn.ID_Transaccion
                            JOIN TipoTransaccion ttn ON ttn.ID_TipoTransaccion = tn.ID_TipoTransaccion
                           WHERE tn.ID_TransaccionOrigen = @ID_TransaccionOrigen AND tn.Estado = N'REGISTRADA'
                             AND ttn.Factor_Kardex <> 0 AND dn.ID_Producto = l.ID_Producto) dev
             WHERE ISNULL(dev.Devuelto, 0) + l.Cantidad > d.Cantidad;
            IF @Msg IS NOT NULL THROW 50001, @Msg, 1;
        END
    END

    -- Compra de una orden: el producto debe estar en la orden y no exceder lo pendiente
    IF @ID_OrdenCompra IS NOT NULL
    BEGIN
        SELECT TOP 1 @Msg = CONCAT(N'''', p.NombreProducto, N''' no está en la orden de compra')
          FROM @Lineas l
          JOIN Producto p ON p.ID_Producto = l.ID_Producto
         WHERE NOT EXISTS (SELECT 1 FROM OrdenCompraDetalle d WHERE d.ID_OrdenCompra = @ID_OrdenCompra AND d.ID_Producto = l.ID_Producto);
        IF @Msg IS NOT NULL THROW 50001, @Msg, 1;

        SELECT TOP 1 @Msg = CONCAT(N'De ''', p.NombreProducto, N''' solo quedan ', d.CantidadPedida - d.CantidadRecibida,
                                   N' unidades pendientes en la orden')
          FROM @Lineas l
          JOIN OrdenCompraDetalle d ON d.ID_OrdenCompra = @ID_OrdenCompra AND d.ID_Producto = l.ID_Producto
          JOIN Producto p ON p.ID_Producto = l.ID_Producto
         WHERE l.Cantidad > d.CantidadPedida - d.CantidadRecibida;
        IF @Msg IS NOT NULL THROW 50001, @Msg, 1;

        UPDATE l SET PrecioUnitario = ISNULL(l.PrecioUnitario, d.CostoUnitario)
          FROM @Lineas l
          JOIN OrdenCompraDetalle d ON d.ID_OrdenCompra = @ID_OrdenCompra AND d.ID_Producto = l.ID_Producto;
    END

    -- Precio sugerido para lo que siga vacío y costo con el que se valoriza el kardex
    -- Compras y ajustes de entrada (inventario inicial) actualizan el costo de compra del producto
    DECLARE @ActualizaCosto BIT = CASE WHEN @CodigoTipo IN (N'COMPRA', N'AJUSTE_ENTRADA') THEN 1 ELSE 0 END;

    UPDATE l
       SET PrecioUnitario = ISNULL(l.PrecioUnitario, CASE WHEN @TipoTercero = N'CLIENTE' THEN p.Precio ELSE p.Costo END)
      FROM @Lineas l
      JOIN Producto p ON p.ID_Producto = l.ID_Producto;

    UPDATE l
       SET CostoUnitario = CASE WHEN @ActualizaCosto = 1 OR @CodigoTipo = N'NC_COMPRA' THEN l.PrecioUnitario ELSE p.Costo END
      FROM @Lineas l
      JOIN Producto p ON p.ID_Producto = l.ID_Producto;

    SELECT TOP 1 @Msg = CONCAT(N'El descuento de ''', p.NombreProducto, N''' es mayor que el importe')
      FROM @Lineas l JOIN Producto p ON p.ID_Producto = l.ID_Producto
     WHERE l.Descuento > l.Cantidad * l.PrecioUnitario;
    IF @Msg IS NOT NULL THROW 50001, @Msg, 1;

    -- ===== 6. Totales (los ajustes no llevan IVA) =====
    DECLARE @Iva DECIMAL(9,4) = 0, @Subtotal DECIMAL(18,2), @Descuento DECIMAL(18,2), @Impuesto DECIMAL(18,2);

    IF @RequiereMotivo = 0
        EXEC sp_Parametro_ObtenerIva @ID_Almacen, @Iva OUTPUT;

    SELECT @Subtotal = SUM(Cantidad * PrecioUnitario), @Descuento = SUM(Descuento) FROM @Lineas;
    SET @Impuesto = ROUND((@Subtotal - @Descuento) * @Iva / 100, 2);

    -- ===== 7. Guardar todo en una transacción: si algo falla no se guarda nada =====
    DECLARE @ID_Transaccion INT, @Numero NVARCHAR(30), @EsEntrada BIT = CASE WHEN @Factor > 0 THEN 1 ELSE 0 END;
    DECLARE @Fila INT = 1, @TotalLineas INT, @ID_Detalle INT,
            @Producto INT, @Cantidad INT, @Precio DECIMAL(18,2), @Costo DECIMAL(18,2), @Desc DECIMAL(18,2), @Motivo INT;

    SELECT @TotalLineas = COUNT(*) FROM @Lineas;

    BEGIN TRY
        BEGIN TRANSACTION;

        EXEC sp_Correlativo_Siguiente @CodigoTipo, @Numero OUTPUT;

        INSERT INTO Transaccion (NumeroDocumento, ID_TipoTransaccion, ID_Usuario, ID_Almacen, ID_Cliente, ID_Proveedor,
                                 ID_OrdenCompra, ID_TransaccionOrigen, Fecha, Referencia, Subtotal, Descuento, Impuesto, Total,
                                 Estado, Observaciones, FechaCreacion)
        VALUES (@Numero, @ID_TipoTransaccion, @ID_Usuario, @ID_Almacen, @ID_Cliente, @ID_Proveedor,
                @ID_OrdenCompra, @ID_TransaccionOrigen, GETDATE(), @Referencia, @Subtotal, @Descuento, @Impuesto,
                @Subtotal - @Descuento + @Impuesto, N'REGISTRADA', @Observaciones, GETDATE());

        SET @ID_Transaccion = SCOPE_IDENTITY();

        WHILE @Fila <= @TotalLineas
        BEGIN
            SELECT @Producto = ID_Producto, @Cantidad = Cantidad, @Precio = PrecioUnitario, @Costo = CostoUnitario,
                   @Desc = Descuento, @Motivo = ID_Motivo
              FROM @Lineas WHERE Fila = @Fila;

            INSERT INTO TransaccionDetalle (ID_Transaccion, ID_Producto, ID_Motivo, Cantidad, PrecioUnitario, CostoUnitario, Descuento, Total)
            VALUES (@ID_Transaccion, @Producto, @Motivo, @Cantidad, @Precio, @Costo, @Desc, ROUND(@Cantidad * @Precio - @Desc, 2));

            SET @ID_Detalle = SCOPE_IDENTITY();

            -- Costo de compra: el producto queda con el costo unitario de esta compra (último costo de compra)
            IF @ActualizaCosto = 1
                UPDATE Producto SET Costo = @Precio WHERE ID_Producto = @Producto;

            -- Factor_Kardex: +1 entra, -1 sale, 0 no mueve inventario (notas de débito)
            IF @Factor <> 0
                EXEC sp_Inventario_RegistrarMovimiento @ID_Almacen, @Producto, @Cantidad, @EsEntrada, @Costo,
                                                       @ID_Usuario, @CodigoTipo, @Numero, @ID_Detalle, NULL;

            SET @Fila += 1;
        END

        -- La orden de compra queda PARCIAL o COMPLETADA
        IF @ID_OrdenCompra IS NOT NULL
        BEGIN
            UPDATE d SET CantidadRecibida = d.CantidadRecibida + l.Cantidad
              FROM OrdenCompraDetalle d
              JOIN @Lineas l ON l.ID_Producto = d.ID_Producto
             WHERE d.ID_OrdenCompra = @ID_OrdenCompra;

            UPDATE OrdenCompra
               SET Estado = CASE WHEN EXISTS (SELECT 1 FROM OrdenCompraDetalle
                                               WHERE ID_OrdenCompra = @ID_OrdenCompra AND CantidadRecibida < CantidadPedida)
                                 THEN N'PARCIAL' ELSE N'COMPLETADA' END,
                   FechaModificacion = GETDATE()
             WHERE ID_OrdenCompra = @ID_OrdenCompra;
        END

        COMMIT;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK;
        THROW;
    END CATCH

    IF @SinResultado = 0
        SELECT @ID_Transaccion AS Value;
END
GO

/* Los documentos no se editan ni se borran: se anulan.
   La anulación genera los movimientos contrarios en el kardex */
CREATE OR ALTER PROCEDURE sp_Transaccion_Anular
    @ID_Transaccion INT,
    @ID_Usuario     INT,
    @Motivo         NVARCHAR(200) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @Estado NVARCHAR(20), @ID_Almacen INT, @Factor INT, @CodigoTipo NVARCHAR(20), @Numero NVARCHAR(30),
            @ID_OrdenCompra INT, @Observaciones NVARCHAR(255), @EsEntrada BIT, @Concepto NVARCHAR(50);

    SELECT @Estado = t.Estado, @ID_Almacen = t.ID_Almacen, @Factor = tt.Factor_Kardex, @CodigoTipo = tt.Codigo,
           @Numero = t.NumeroDocumento, @ID_OrdenCompra = t.ID_OrdenCompra, @Observaciones = t.Observaciones
      FROM Transaccion t
      JOIN TipoTransaccion tt ON tt.ID_TipoTransaccion = t.ID_TipoTransaccion
     WHERE t.ID_Transaccion = @ID_Transaccion;

    IF @Estado IS NULL
        THROW 50404, N'La transacción no existe', 1;

    IF @Estado = N'ANULADA'
        THROW 50001, N'El documento ya está anulado', 1;

    EXEC sp_Usuario_ValidarAlmacen @ID_Usuario, @ID_Almacen;

    IF EXISTS (SELECT 1 FROM Transaccion WHERE ID_TransaccionOrigen = @ID_Transaccion AND Estado = N'REGISTRADA')
        THROW 50001, N'Primero anule las notas de crédito o débito que hacen referencia a este documento', 1;

    -- Movimiento contrario: si la venta sacó producto, la anulación lo regresa
    SET @EsEntrada = CASE WHEN @Factor < 0 THEN 1 ELSE 0 END;
    SET @Concepto = CONCAT(N'ANULACION ', @CodigoTipo);

    DECLARE @ID_Detalle INT = 0, @Producto INT, @Cantidad INT, @Costo DECIMAL(18,2);

    BEGIN TRY
        BEGIN TRANSACTION;

        IF @Factor <> 0
        BEGIN
            WHILE 1 = 1
            BEGIN
                SELECT TOP 1 @ID_Detalle = ID_Detalle, @Producto = ID_Producto, @Cantidad = Cantidad, @Costo = CostoUnitario
                  FROM TransaccionDetalle
                 WHERE ID_Transaccion = @ID_Transaccion AND ID_Detalle > @ID_Detalle
                 ORDER BY ID_Detalle;

                IF @@ROWCOUNT = 0 BREAK;

                EXEC sp_Inventario_RegistrarMovimiento @ID_Almacen, @Producto, @Cantidad, @EsEntrada, @Costo,
                                                       @ID_Usuario, @Concepto, @Numero, @ID_Detalle, NULL;
            END
        END

        -- Si era una compra de una orden, la cantidad vuelve a quedar pendiente
        IF @ID_OrdenCompra IS NOT NULL
        BEGIN
            UPDATE d SET CantidadRecibida = d.CantidadRecibida - td.Cantidad
              FROM OrdenCompraDetalle d
              JOIN TransaccionDetalle td ON td.ID_Producto = d.ID_Producto AND td.ID_Transaccion = @ID_Transaccion
             WHERE d.ID_OrdenCompra = @ID_OrdenCompra;

            UPDATE OrdenCompra
               SET Estado = CASE
                               WHEN NOT EXISTS (SELECT 1 FROM OrdenCompraDetalle WHERE ID_OrdenCompra = @ID_OrdenCompra AND CantidadRecibida > 0)
                                   THEN N'PENDIENTE'
                               WHEN NOT EXISTS (SELECT 1 FROM OrdenCompraDetalle WHERE ID_OrdenCompra = @ID_OrdenCompra AND CantidadRecibida < CantidadPedida)
                                   THEN N'COMPLETADA'
                               ELSE N'PARCIAL'
                            END,
                   FechaModificacion = GETDATE()
             WHERE ID_OrdenCompra = @ID_OrdenCompra AND Estado <> N'CANCELADA';
        END

        UPDATE Transaccion
           SET Estado = N'ANULADA',
               ID_UsuarioAnula = @ID_Usuario,
               FechaAnulacion = GETDATE(),
               Observaciones = CASE WHEN NULLIF(LTRIM(@Motivo), N'') IS NULL THEN @Observaciones
                                    ELSE LEFT(CONCAT(@Observaciones, CASE WHEN @Observaciones IS NULL THEN N'' ELSE N' | ' END,
                                                     N'Anulado: ', @Motivo), 255) END
         WHERE ID_Transaccion = @ID_Transaccion;

        COMMIT;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK;
        THROW;
    END CATCH
END
GO
