/* =====================================================================
   01 - PROCEDIMIENTOS INTERNOS
   Los usan otros procedimientos (no se llaman desde la API).
   Convención de errores:
     THROW 50001..50399 -> regla de negocio (la API responde 400)
     THROW 50404        -> registro no encontrado (la API responde 404)
   ===================================================================== */

-- Devuelve el siguiente número de documento (ej: VEN-000015) y lo deja reservado
CREATE OR ALTER PROCEDURE sp_Correlativo_Siguiente
    @Clave  NVARCHAR(30),
    @Numero NVARCHAR(30) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Prefijo NVARCHAR(10), @Ultimo INT, @Msg NVARCHAR(200);

    -- El UPDATE bloquea la fila: dos usuarios nunca obtienen el mismo número
    UPDATE Correlativo
       SET @Ultimo = UltimoNumero = UltimoNumero + 1,
           @Prefijo = Prefijo
     WHERE Clave = @Clave AND Estado = 1;

    IF @Ultimo IS NULL
    BEGIN
        SET @Msg = CONCAT(N'No existe un correlativo activo con la clave ', @Clave);
        THROW 50001, @Msg, 1;
    END

    SET @Numero = CONCAT(@Prefijo, N'-', RIGHT(CONCAT(N'000000', @Ultimo), 6));
END
GO

-- IVA del almacén; si el almacén no tiene uno propio, usa el global (ID_Almacen = NULL)
CREATE OR ALTER PROCEDURE sp_Parametro_ObtenerIva
    @ID_Almacen INT,
    @Iva        DECIMAL(9,4) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Valor NVARCHAR(255);

    SELECT TOP 1 @Valor = Valor
      FROM ParametroSistema
     WHERE Clave = N'IVA_PORCENTAJE' AND Estado = 1
       AND (ID_Almacen IS NULL OR ID_Almacen = @ID_Almacen)
     ORDER BY CASE WHEN ID_Almacen IS NULL THEN 1 ELSE 0 END;

    SET @Iva = ISNULL(TRY_CAST(@Valor AS DECIMAL(9,4)), 0);
END
GO

-- Verifica que el usuario y el almacén estén activos y que el usuario tenga asignado el almacén.
-- El rol Administrador puede trabajar en todos los almacenes.
CREATE OR ALTER PROCEDURE sp_Usuario_ValidarAlmacen
    @ID_Usuario INT,
    @ID_Almacen INT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @NombreRol NVARCHAR(50), @NombreAlmacen NVARCHAR(100), @Msg NVARCHAR(300);

    SELECT @NombreRol = r.NombreRol
      FROM Usuario u
      JOIN Rol r ON r.ID_Rol = u.ID_Rol
     WHERE u.ID_Usuario = @ID_Usuario AND u.Estado = 1;

    IF @NombreRol IS NULL
        THROW 50001, N'El usuario no existe o está inactivo', 1;

    SELECT @NombreAlmacen = Nombre FROM Almacen WHERE ID_Almacen = @ID_Almacen AND Estado = 1;

    IF @NombreAlmacen IS NULL
        THROW 50001, N'El almacén no existe o está inactivo', 1;

    IF @NombreRol <> N'Administrador'
       AND NOT EXISTS (SELECT 1 FROM UsuarioAlmacen WHERE ID_Usuario = @ID_Usuario AND ID_Almacen = @ID_Almacen)
    BEGIN
        SET @Msg = CONCAT(N'El usuario no tiene acceso al almacén ''', @NombreAlmacen, N'''');
        THROW 50001, @Msg, 1;
    END
END
GO

-- ÚNICO procedimiento que modifica el stock. Cada cambio deja su fila en el kardex (HistorialMovimientos)
CREATE OR ALTER PROCEDURE sp_Inventario_RegistrarMovimiento
    @ID_Almacen              INT,
    @ID_Producto             INT,
    @Cantidad                INT,
    @EsEntrada               BIT,
    @CostoUnitario           DECIMAL(18,2),
    @ID_Usuario              INT,
    @Concepto                NVARCHAR(50),
    @Documento               NVARCHAR(30),
    @ID_Detalle              INT = NULL,
    @ID_DetalleTransferencia INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @SaldoAnterior INT, @SaldoActual INT, @SaldoProducto INT, @Msg NVARCHAR(400);

    IF @Cantidad IS NULL OR @Cantidad <= 0
        THROW 50001, N'La cantidad debe ser mayor a cero', 1;

    -- Si el producto nunca ha estado en este almacén, se crea su fila con stock 0
    IF NOT EXISTS (SELECT 1 FROM InventarioAlmacen WHERE ID_Almacen = @ID_Almacen AND ID_Producto = @ID_Producto)
        INSERT INTO InventarioAlmacen (ID_Almacen, ID_Producto, StockActual, StockMinimo, StockMaximo)
        VALUES (@ID_Almacen, @ID_Producto, 0, 0, 0);

    SELECT @SaldoAnterior = StockActual
      FROM InventarioAlmacen WITH (UPDLOCK)
     WHERE ID_Almacen = @ID_Almacen AND ID_Producto = @ID_Producto;

    SET @SaldoActual = CASE WHEN @EsEntrada = 1 THEN @SaldoAnterior + @Cantidad ELSE @SaldoAnterior - @Cantidad END;

    IF @SaldoActual < 0
    BEGIN
        SELECT @Msg = CONCAT(N'Stock insuficiente de ''', NombreProducto, N''': hay ', @SaldoAnterior, N' y se necesitan ', @Cantidad)
          FROM Producto WHERE ID_Producto = @ID_Producto;
        THROW 50001, @Msg, 1;
    END

    UPDATE InventarioAlmacen
       SET StockActual = @SaldoActual,
           FechaUltimoMovimiento = GETDATE()
     WHERE ID_Almacen = @ID_Almacen AND ID_Producto = @ID_Producto;

    -- Existencia total del producto sumando todos los almacenes
    SELECT @SaldoProducto = SUM(StockActual) FROM InventarioAlmacen WHERE ID_Producto = @ID_Producto;

    INSERT INTO HistorialMovimientos
        (ID_Producto, ID_Almacen, ID_Usuario, ID_Detalle, ID_DetalleTransferencia, TipoMovimiento, Concepto, Documento,
         Cantidad, CostoUnitario, SaldoAnterior_Almacen, SaldoActual_Almacen, SaldoActual_Producto, FechaRegistro)
    VALUES
        (@ID_Producto, @ID_Almacen, @ID_Usuario, @ID_Detalle, @ID_DetalleTransferencia,
         CASE WHEN @EsEntrada = 1 THEN N'ENTRADA' ELSE N'SALIDA' END, @Concepto, @Documento,
         @Cantidad, @CostoUnitario, @SaldoAnterior, @SaldoActual, @SaldoProducto, GETDATE());
END
GO
