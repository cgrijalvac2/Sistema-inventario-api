/* =====================================================================
   07 - TRANSFERENCIAS ENTRE ALMACENES
   Flujo: se crea (ENVIADA, sale del origen) -> el destino la RECIBE (entra al destino)
          o la RECHAZA (la mercadería regresa al origen)
   ===================================================================== */

CREATE OR ALTER PROCEDURE sp_Transferencia_Listar
    @ID_Almacen INT = NULL,
    @Estado     NVARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT t.ID_Transferencia, t.NumeroTransferencia, t.ID_AlmacenOrigen, ao.Nombre AS AlmacenOrigen,
           t.ID_AlmacenDestino, ad.Nombre AS AlmacenDestino, ue.NombreCompleto AS UsuarioEnvia, t.FechaEnvio,
           ur.NombreCompleto AS UsuarioRecibe, t.FechaRecepcion, t.MotivoRechazo, t.Estado, t.Observaciones
      FROM Transferencia t
      JOIN Almacen ao ON ao.ID_Almacen = t.ID_AlmacenOrigen
      JOIN Almacen ad ON ad.ID_Almacen = t.ID_AlmacenDestino
      JOIN Usuario ue ON ue.ID_Usuario = t.ID_UsuarioEnvia
      LEFT JOIN Usuario ur ON ur.ID_Usuario = t.ID_UsuarioRecibe
     WHERE (@ID_Almacen IS NULL OR t.ID_AlmacenOrigen = @ID_Almacen OR t.ID_AlmacenDestino = @ID_Almacen)
       AND (@Estado IS NULL OR t.Estado = UPPER(@Estado))
     ORDER BY t.FechaEnvio DESC;
END
GO

CREATE OR ALTER PROCEDURE sp_Transferencia_Obtener
    @ID_Transferencia INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT t.ID_Transferencia, t.NumeroTransferencia, t.ID_AlmacenOrigen, ao.Nombre AS AlmacenOrigen,
           t.ID_AlmacenDestino, ad.Nombre AS AlmacenDestino, ue.NombreCompleto AS UsuarioEnvia, t.FechaEnvio,
           ur.NombreCompleto AS UsuarioRecibe, t.FechaRecepcion, t.MotivoRechazo, t.Estado, t.Observaciones
      FROM Transferencia t
      JOIN Almacen ao ON ao.ID_Almacen = t.ID_AlmacenOrigen
      JOIN Almacen ad ON ad.ID_Almacen = t.ID_AlmacenDestino
      JOIN Usuario ue ON ue.ID_Usuario = t.ID_UsuarioEnvia
      LEFT JOIN Usuario ur ON ur.ID_Usuario = t.ID_UsuarioRecibe
     WHERE t.ID_Transferencia = @ID_Transferencia;
END
GO

CREATE OR ALTER PROCEDURE sp_Transferencia_Detalle
    @ID_Transferencia INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT d.ID_DetalleTransferencia, d.ID_Producto, p.CodigoProducto, p.NombreProducto, d.Cantidad
      FROM TransferenciaDetalle d
      JOIN Producto p ON p.ID_Producto = d.ID_Producto
     WHERE d.ID_Transferencia = @ID_Transferencia
     ORDER BY d.ID_DetalleTransferencia;
END
GO

/* @Detalles es JSON: [{"ID_Producto":1,"Cantidad":3}]
   Devuelve el ID de la transferencia */
CREATE OR ALTER PROCEDURE sp_Transferencia_Registrar
    @ID_AlmacenOrigen  INT,
    @ID_AlmacenDestino INT,
    @ID_Usuario        INT,
    @Observaciones     NVARCHAR(255) = NULL,
    @Detalles          NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    DECLARE @Msg NVARCHAR(400), @ID_Transferencia INT, @Numero NVARCHAR(30);

    IF @ID_AlmacenOrigen = @ID_AlmacenDestino
        THROW 50001, N'El almacén de origen y el de destino deben ser distintos', 1;

    EXEC sp_Usuario_ValidarAlmacen @ID_Usuario, @ID_AlmacenOrigen;

    IF NOT EXISTS (SELECT 1 FROM Almacen WHERE ID_Almacen = @ID_AlmacenDestino AND Estado = 1)
        THROW 50001, N'El almacén de destino no existe o está inactivo', 1;

    IF ISJSON(@Detalles) = 0
        THROW 50001, N'El detalle no tiene un formato válido', 1;

    DECLARE @Lineas TABLE (Fila INT IDENTITY(1,1), ID_Producto INT, Cantidad INT);

    INSERT INTO @Lineas (ID_Producto, Cantidad)
    SELECT ID_Producto, Cantidad
      FROM OPENJSON(@Detalles) WITH (ID_Producto INT '$.ID_Producto', Cantidad INT '$.Cantidad');

    IF NOT EXISTS (SELECT 1 FROM @Lineas)
        THROW 50001, N'La transferencia debe tener al menos un producto', 1;

    IF EXISTS (SELECT ID_Producto FROM @Lineas GROUP BY ID_Producto HAVING COUNT(*) > 1)
        THROW 50001, N'Un producto no se puede repetir en la misma transferencia', 1;

    IF EXISTS (SELECT 1 FROM @Lineas WHERE Cantidad IS NULL OR Cantidad <= 0)
        THROW 50001, N'La cantidad debe ser mayor a cero', 1;

    SELECT TOP 1 @Msg = CONCAT(N'El producto ', l.ID_Producto, N' no existe o está inactivo')
      FROM @Lineas l
      LEFT JOIN Producto p ON p.ID_Producto = l.ID_Producto AND p.Estado = 1
     WHERE p.ID_Producto IS NULL;
    IF @Msg IS NOT NULL THROW 50001, @Msg, 1;

    DECLARE @Fila INT = 1, @TotalLineas INT, @ID_Detalle INT, @Producto INT, @Cantidad INT, @Costo DECIMAL(18,2);
    SELECT @TotalLineas = COUNT(*) FROM @Lineas;

    BEGIN TRY
        BEGIN TRANSACTION;

        EXEC sp_Correlativo_Siguiente N'TRANSFERENCIA', @Numero OUTPUT;

        INSERT INTO Transferencia (NumeroTransferencia, ID_AlmacenOrigen, ID_AlmacenDestino, ID_UsuarioEnvia, FechaEnvio, Estado, Observaciones)
        VALUES (@Numero, @ID_AlmacenOrigen, @ID_AlmacenDestino, @ID_Usuario, GETDATE(), N'ENVIADA', @Observaciones);

        SET @ID_Transferencia = SCOPE_IDENTITY();

        WHILE @Fila <= @TotalLineas
        BEGIN
            SELECT @Producto = l.ID_Producto, @Cantidad = l.Cantidad, @Costo = p.Costo
              FROM @Lineas l JOIN Producto p ON p.ID_Producto = l.ID_Producto
             WHERE l.Fila = @Fila;

            INSERT INTO TransferenciaDetalle (ID_Transferencia, ID_Producto, Cantidad)
            VALUES (@ID_Transferencia, @Producto, @Cantidad);

            SET @ID_Detalle = SCOPE_IDENTITY();

            -- Sale del almacén origen (valida que haya stock suficiente)
            EXEC sp_Inventario_RegistrarMovimiento @ID_AlmacenOrigen, @Producto, @Cantidad, 0, @Costo,
                                                   @ID_Usuario, N'TRANSFERENCIA ENVIADA', @Numero, NULL, @ID_Detalle;
            SET @Fila += 1;
        END

        COMMIT;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK;
        THROW;
    END CATCH

    SELECT @ID_Transferencia AS Value;
END
GO

/* @Recibir = 1: entra al almacén destino
   @Recibir = 0: se rechaza y regresa al almacén origen (requiere @Motivo)
   Quien recibe o rechaza debe tener acceso al almacén destino */
CREATE OR ALTER PROCEDURE sp_Transferencia_Finalizar
    @ID_Transferencia INT,
    @ID_Usuario       INT,
    @Recibir          BIT,
    @Motivo           NVARCHAR(255) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    DECLARE @Estado NVARCHAR(20), @Origen INT, @Destino INT, @Numero NVARCHAR(30), @Msg NVARCHAR(200),
            @ID_Almacen INT, @Concepto NVARCHAR(50);

    SELECT @Estado = Estado, @Origen = ID_AlmacenOrigen, @Destino = ID_AlmacenDestino, @Numero = NumeroTransferencia
      FROM Transferencia WHERE ID_Transferencia = @ID_Transferencia;

    IF @Estado IS NULL
        THROW 50404, N'La transferencia no existe', 1;

    IF @Estado <> N'ENVIADA'
    BEGIN
        SET @Msg = CONCAT(N'La transferencia ya fue ', @Estado);
        THROW 50001, @Msg, 1;
    END

    EXEC sp_Usuario_ValidarAlmacen @ID_Usuario, @Destino;

    IF @Recibir = 0 AND NULLIF(LTRIM(@Motivo), N'') IS NULL
        THROW 50001, N'Debe indicar el motivo del rechazo', 1;

    SET @ID_Almacen = CASE WHEN @Recibir = 1 THEN @Destino ELSE @Origen END;
    SET @Concepto = CASE WHEN @Recibir = 1 THEN N'TRANSFERENCIA RECIBIDA' ELSE N'TRANSFERENCIA RECHAZADA' END;

    DECLARE @ID_Detalle INT = 0, @Producto INT, @Cantidad INT, @Costo DECIMAL(18,2);

    BEGIN TRY
        BEGIN TRANSACTION;

        WHILE 1 = 1
        BEGIN
            SELECT TOP 1 @ID_Detalle = d.ID_DetalleTransferencia, @Producto = d.ID_Producto, @Cantidad = d.Cantidad, @Costo = p.Costo
              FROM TransferenciaDetalle d
              JOIN Producto p ON p.ID_Producto = d.ID_Producto
             WHERE d.ID_Transferencia = @ID_Transferencia AND d.ID_DetalleTransferencia > @ID_Detalle
             ORDER BY d.ID_DetalleTransferencia;

            IF @@ROWCOUNT = 0 BREAK;

            EXEC sp_Inventario_RegistrarMovimiento @ID_Almacen, @Producto, @Cantidad, 1, @Costo,
                                                   @ID_Usuario, @Concepto, @Numero, NULL, @ID_Detalle;
        END

        UPDATE Transferencia
           SET Estado = CASE WHEN @Recibir = 1 THEN N'RECIBIDA' ELSE N'RECHAZADA' END,
               ID_UsuarioRecibe = @ID_Usuario,
               FechaRecepcion = GETDATE(),
               MotivoRechazo = CASE WHEN @Recibir = 1 THEN NULL ELSE @Motivo END
         WHERE ID_Transferencia = @ID_Transferencia;

        COMMIT;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK;
        THROW;
    END CATCH
END
GO
