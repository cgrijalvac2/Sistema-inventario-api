/* =====================================================================
   03 - CONFIGURACIÓN: parámetros del sistema, correlativos y tipos de transacción
   ===================================================================== */

-- ---------- PARÁMETROS DEL SISTEMA ----------

CREATE OR ALTER PROCEDURE sp_ParametroSistema_Listar
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ID_Parametro, ID_Almacen, Clave, Valor, Descripcion, Estado, FechaModificacion
      FROM ParametroSistema
     ORDER BY Clave, ID_Almacen;
END
GO

CREATE OR ALTER PROCEDURE sp_ParametroSistema_Obtener
    @ID_Parametro INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ID_Parametro, ID_Almacen, Clave, Valor, Descripcion, Estado, FechaModificacion
      FROM ParametroSistema
     WHERE ID_Parametro = @ID_Parametro;
END
GO

CREATE OR ALTER PROCEDURE sp_ParametroSistema_Insertar
    @ID_Almacen  INT = NULL,
    @Clave       NVARCHAR(50),
    @Valor       NVARCHAR(255),
    @Descripcion NVARCHAR(255) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @ID INT;

    -- La misma clave no se repite para el mismo almacén (ni dos veces como global)
    IF EXISTS (SELECT 1 FROM ParametroSistema WHERE Clave = UPPER(@Clave) AND ISNULL(ID_Almacen, 0) = ISNULL(@ID_Almacen, 0))
        THROW 50001, N'Ya existe ese parámetro para el mismo almacén', 1;

    IF @ID_Almacen IS NOT NULL AND NOT EXISTS (SELECT 1 FROM Almacen WHERE ID_Almacen = @ID_Almacen)
        THROW 50001, N'El almacén no existe', 1;

    INSERT INTO ParametroSistema (ID_Almacen, Clave, Valor, Descripcion, Estado, FechaModificacion)
    VALUES (@ID_Almacen, UPPER(@Clave), @Valor, @Descripcion, 1, GETDATE());

    SET @ID = SCOPE_IDENTITY();
    EXEC sp_ParametroSistema_Obtener @ID;
END
GO

CREATE OR ALTER PROCEDURE sp_ParametroSistema_Actualizar
    @ID_Parametro INT,
    @ID_Almacen   INT = NULL,
    @Clave        NVARCHAR(50),
    @Valor        NVARCHAR(255),
    @Descripcion  NVARCHAR(255) = NULL,
    @Estado       BIT = 1
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM ParametroSistema WHERE ID_Parametro = @ID_Parametro)
        THROW 50404, N'El parámetro no existe', 1;

    IF EXISTS (SELECT 1 FROM ParametroSistema
                WHERE Clave = UPPER(@Clave) AND ISNULL(ID_Almacen, 0) = ISNULL(@ID_Almacen, 0) AND ID_Parametro <> @ID_Parametro)
        THROW 50001, N'Ya existe ese parámetro para el mismo almacén', 1;

    UPDATE ParametroSistema
       SET ID_Almacen = @ID_Almacen, Clave = UPPER(@Clave), Valor = @Valor, Descripcion = @Descripcion,
           Estado = @Estado, FechaModificacion = GETDATE()
     WHERE ID_Parametro = @ID_Parametro;
END
GO

CREATE OR ALTER PROCEDURE sp_ParametroSistema_Eliminar
    @ID_Parametro INT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM ParametroSistema WHERE ID_Parametro = @ID_Parametro)
        THROW 50404, N'El parámetro no existe', 1;

    UPDATE ParametroSistema SET Estado = 0, FechaModificacion = GETDATE() WHERE ID_Parametro = @ID_Parametro;
END
GO

-- ---------- CORRELATIVOS ----------
-- Solo se consultan y modifican: las claves las usa el sistema, por eso no se crean ni se borran

CREATE OR ALTER PROCEDURE sp_Correlativo_Listar
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ID_Correlativo, ID_TipoTransaccion, Clave, Prefijo, UltimoNumero, Estado
      FROM Correlativo
     ORDER BY ID_Correlativo;
END
GO

CREATE OR ALTER PROCEDURE sp_Correlativo_Obtener
    @ID_Correlativo INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ID_Correlativo, ID_TipoTransaccion, Clave, Prefijo, UltimoNumero, Estado
      FROM Correlativo
     WHERE ID_Correlativo = @ID_Correlativo;
END
GO

CREATE OR ALTER PROCEDURE sp_Correlativo_Actualizar
    @ID_Correlativo INT,
    @Prefijo        NVARCHAR(10),
    @UltimoNumero   INT,
    @Estado         BIT = 1
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM Correlativo WHERE ID_Correlativo = @ID_Correlativo)
        THROW 50404, N'El correlativo no existe', 1;

    IF @UltimoNumero < 0
        THROW 50001, N'El último número no puede ser negativo', 1;

    UPDATE Correlativo
       SET Prefijo = UPPER(@Prefijo), UltimoNumero = @UltimoNumero, Estado = @Estado
     WHERE ID_Correlativo = @ID_Correlativo;
END
GO

-- ---------- TIPOS DE TRANSACCIÓN ----------
-- Solo se cambia nombre, descripción y estado: Factor_Kardex y las reglas definen cómo se mueve el inventario

CREATE OR ALTER PROCEDURE sp_TipoTransaccion_Listar
    @SoloActivos BIT = 0
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ID_TipoTransaccion, Codigo, Nombre, Descripcion, Factor_Kardex, TipoTercero,
           RequiereDocumentoOrigen, RequiereMotivo, Estado
      FROM TipoTransaccion
     WHERE @SoloActivos = 0 OR Estado = 1
     ORDER BY ID_TipoTransaccion;
END
GO

CREATE OR ALTER PROCEDURE sp_TipoTransaccion_Obtener
    @ID_TipoTransaccion INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ID_TipoTransaccion, Codigo, Nombre, Descripcion, Factor_Kardex, TipoTercero,
           RequiereDocumentoOrigen, RequiereMotivo, Estado
      FROM TipoTransaccion
     WHERE ID_TipoTransaccion = @ID_TipoTransaccion;
END
GO

CREATE OR ALTER PROCEDURE sp_TipoTransaccion_Actualizar
    @ID_TipoTransaccion INT,
    @Nombre             NVARCHAR(50),
    @Descripcion        NVARCHAR(255) = NULL,
    @Estado             BIT = 1
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM TipoTransaccion WHERE ID_TipoTransaccion = @ID_TipoTransaccion)
        THROW 50404, N'El tipo de transacción no existe', 1;

    UPDATE TipoTransaccion
       SET Nombre = @Nombre, Descripcion = @Descripcion, Estado = @Estado
     WHERE ID_TipoTransaccion = @ID_TipoTransaccion;
END
GO
