/* =====================================================================
   02 - SEGURIDAD: login, usuarios, roles y permisos
   ===================================================================== */

-- ---------- LOGIN ----------

-- Devuelve el usuario con su hash. La contraseña se valida en C# con PasswordHasher
CREATE OR ALTER PROCEDURE sp_Auth_ObtenerUsuario
    @CorreoElectronico NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT u.ID_Usuario, u.NombreCompleto, u.CorreoElectronico, u.PasswordHash, u.ID_Rol,
           r.NombreRol, u.Estado, r.Estado AS RolEstado
      FROM Usuario u
      JOIN Rol r ON r.ID_Rol = u.ID_Rol
     WHERE u.CorreoElectronico = LOWER(@CorreoElectronico);
END
GO

CREATE OR ALTER PROCEDURE sp_Auth_RegistrarAcceso
    @ID_Usuario INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Usuario SET UltimoAcceso = GETDATE() WHERE ID_Usuario = @ID_Usuario;
END
GO

-- Permisos activos del rol del usuario (el frontend arma el menú con ellos)
CREATE OR ALTER PROCEDURE sp_Auth_Permisos
    @ID_Usuario INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT p.ID_Permiso, p.Codigo, p.Nombre
      FROM Usuario u
      JOIN RolPermiso rp ON rp.ID_Rol = u.ID_Rol
      JOIN Permiso p ON p.ID_Permiso = rp.ID_Permiso AND p.Estado = 1
     WHERE u.ID_Usuario = @ID_Usuario
     ORDER BY p.Codigo;
END
GO

-- Almacenes en los que puede trabajar: el Administrador todos, los demás solo los asignados
CREATE OR ALTER PROCEDURE sp_Auth_Almacenes
    @ID_Usuario INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT a.ID_Almacen, a.Codigo, a.Nombre
      FROM Almacen a
     WHERE a.Estado = 1
       AND (EXISTS (SELECT 1 FROM Usuario u JOIN Rol r ON r.ID_Rol = u.ID_Rol
                     WHERE u.ID_Usuario = @ID_Usuario AND r.NombreRol = N'Administrador')
            OR EXISTS (SELECT 1 FROM UsuarioAlmacen ua WHERE ua.ID_Usuario = @ID_Usuario AND ua.ID_Almacen = a.ID_Almacen))
     ORDER BY a.Nombre;
END
GO

-- ---------- USUARIOS ----------

CREATE OR ALTER PROCEDURE sp_Usuario_Listar
AS
BEGIN
    SET NOCOUNT ON;
    SELECT u.ID_Usuario, u.NombreCompleto, u.CorreoElectronico, u.ID_Rol, r.NombreRol,
           u.Estado, u.FechaCreacion, u.UltimoAcceso
      FROM Usuario u
      JOIN Rol r ON r.ID_Rol = u.ID_Rol
     ORDER BY u.NombreCompleto;
END
GO

CREATE OR ALTER PROCEDURE sp_Usuario_Obtener
    @ID_Usuario INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT u.ID_Usuario, u.NombreCompleto, u.CorreoElectronico, u.ID_Rol, r.NombreRol,
           u.Estado, u.FechaCreacion, u.UltimoAcceso
      FROM Usuario u
      JOIN Rol r ON r.ID_Rol = u.ID_Rol
     WHERE u.ID_Usuario = @ID_Usuario;
END
GO

-- Almacenes asignados (de un usuario o de todos si @ID_Usuario es NULL)
CREATE OR ALTER PROCEDURE sp_Usuario_Almacenes
    @ID_Usuario INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ua.ID_Usuario, a.ID_Almacen, a.Codigo, a.Nombre
      FROM UsuarioAlmacen ua
      JOIN Almacen a ON a.ID_Almacen = ua.ID_Almacen
     WHERE @ID_Usuario IS NULL OR ua.ID_Usuario = @ID_Usuario
     ORDER BY a.Nombre;
END
GO

-- Validaciones comunes de insertar y actualizar usuario
CREATE OR ALTER PROCEDURE sp_Usuario_Validar
    @ID_Usuario        INT,
    @CorreoElectronico NVARCHAR(100),
    @ID_Rol            INT,
    @Almacenes         NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM Usuario WHERE CorreoElectronico = LOWER(@CorreoElectronico) AND ID_Usuario <> @ID_Usuario)
        THROW 50001, N'Ya existe un usuario con ese correo electrónico', 1;

    IF NOT EXISTS (SELECT 1 FROM Rol WHERE ID_Rol = @ID_Rol AND Estado = 1)
        THROW 50001, N'El rol no existe o está inactivo', 1;

    IF ISJSON(@Almacenes) = 0
        THROW 50001, N'La lista de almacenes no tiene un formato válido', 1;

    IF EXISTS (SELECT 1 FROM OPENJSON(@Almacenes) j
                LEFT JOIN Almacen a ON a.ID_Almacen = TRY_CAST(j.value AS INT) AND a.Estado = 1
               WHERE a.ID_Almacen IS NULL)
        THROW 50001, N'Uno o más almacenes no existen o están inactivos', 1;
END
GO

-- @Almacenes es un arreglo JSON con los ID, ej: '[1,2]'
CREATE OR ALTER PROCEDURE sp_Usuario_Insertar
    @NombreCompleto    NVARCHAR(150),
    @CorreoElectronico NVARCHAR(100),
    @PasswordHash      NVARCHAR(255),
    @ID_Rol            INT,
    @Almacenes         NVARCHAR(MAX) = N'[]',
    @Estado BIT = 1   -- 1 = activo, 0 = inactivo
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    DECLARE @ID_Usuario INT;

    EXEC sp_Usuario_Validar 0, @CorreoElectronico, @ID_Rol, @Almacenes;

    BEGIN TRANSACTION;

    INSERT INTO Usuario (NombreCompleto, CorreoElectronico, PasswordHash, ID_Rol, Estado, FechaCreacion)
    VALUES (@NombreCompleto, LOWER(@CorreoElectronico), @PasswordHash, @ID_Rol, @Estado, GETDATE());

    SET @ID_Usuario = SCOPE_IDENTITY();

    INSERT INTO UsuarioAlmacen (ID_Usuario, ID_Almacen)
    SELECT DISTINCT @ID_Usuario, CAST(value AS INT) FROM OPENJSON(@Almacenes);

    COMMIT;

    SELECT @ID_Usuario AS Value;
END
GO

-- @PasswordHash NULL = conserva la contraseña actual
CREATE OR ALTER PROCEDURE sp_Usuario_Actualizar
    @ID_Usuario        INT,
    @NombreCompleto    NVARCHAR(150),
    @CorreoElectronico NVARCHAR(100),
    @PasswordHash      NVARCHAR(255) = NULL,
    @ID_Rol            INT,
    @Almacenes         NVARCHAR(MAX) = N'[]',
    @Estado BIT = NULL   -- NULL = no cambia el estado
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF NOT EXISTS (SELECT 1 FROM Usuario WHERE ID_Usuario = @ID_Usuario)
        THROW 50404, N'El usuario no existe', 1;

    EXEC sp_Usuario_Validar @ID_Usuario, @CorreoElectronico, @ID_Rol, @Almacenes;

    BEGIN TRANSACTION;

    UPDATE Usuario
       SET NombreCompleto = @NombreCompleto,
           CorreoElectronico = LOWER(@CorreoElectronico),
           PasswordHash = ISNULL(@PasswordHash, PasswordHash),
           ID_Rol = @ID_Rol,
           Estado = ISNULL(@Estado, Estado)
     WHERE ID_Usuario = @ID_Usuario;

    -- Se reemplazan los almacenes asignados
    DELETE FROM UsuarioAlmacen WHERE ID_Usuario = @ID_Usuario;

    INSERT INTO UsuarioAlmacen (ID_Usuario, ID_Almacen)
    SELECT DISTINCT @ID_Usuario, CAST(value AS INT) FROM OPENJSON(@Almacenes);

    COMMIT;
END
GO

-- Borrado lógico (@Estado = 0) o reactivación (@Estado = 1)
CREATE OR ALTER PROCEDURE sp_Usuario_CambiarEstado
    @ID_Usuario INT,
    @Estado     BIT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM Usuario WHERE ID_Usuario = @ID_Usuario)
        THROW 50404, N'El usuario no existe', 1;

    UPDATE Usuario SET Estado = @Estado WHERE ID_Usuario = @ID_Usuario;
END
GO

-- ---------- ROLES ----------

CREATE OR ALTER PROCEDURE sp_Rol_Listar
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ID_Rol, NombreRol, Descripcion, Estado FROM Rol ORDER BY NombreRol;
END
GO

CREATE OR ALTER PROCEDURE sp_Rol_Obtener
    @ID_Rol INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ID_Rol, NombreRol, Descripcion, Estado FROM Rol WHERE ID_Rol = @ID_Rol;
END
GO

-- Permisos asignados (de un rol o de todos si @ID_Rol es NULL)
CREATE OR ALTER PROCEDURE sp_Rol_Permisos
    @ID_Rol INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT rp.ID_Rol, p.ID_Permiso, p.Codigo, p.Nombre
      FROM RolPermiso rp
      JOIN Permiso p ON p.ID_Permiso = rp.ID_Permiso
     WHERE @ID_Rol IS NULL OR rp.ID_Rol = @ID_Rol
     ORDER BY p.Codigo;
END
GO

CREATE OR ALTER PROCEDURE sp_Rol_Validar
    @ID_Rol    INT,
    @NombreRol NVARCHAR(50),
    @Permisos  NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM Rol WHERE NombreRol = @NombreRol AND ID_Rol <> @ID_Rol)
        THROW 50001, N'Ya existe un rol con ese nombre', 1;

    IF ISJSON(@Permisos) = 0
        THROW 50001, N'La lista de permisos no tiene un formato válido', 1;

    IF EXISTS (SELECT 1 FROM OPENJSON(@Permisos) j
                LEFT JOIN Permiso p ON p.ID_Permiso = TRY_CAST(j.value AS INT)
               WHERE p.ID_Permiso IS NULL)
        THROW 50001, N'Uno o más permisos no existen', 1;
END
GO

-- @Permisos es un arreglo JSON con los ID, ej: '[1,5,8]'
CREATE OR ALTER PROCEDURE sp_Rol_Insertar
    @NombreRol   NVARCHAR(50),
    @Descripcion NVARCHAR(255) = NULL,
    @Permisos    NVARCHAR(MAX) = N'[]',
    @Estado BIT = 1   -- 1 = activo, 0 = inactivo
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    DECLARE @ID_Rol INT;

    EXEC sp_Rol_Validar 0, @NombreRol, @Permisos;

    BEGIN TRANSACTION;

    INSERT INTO Rol (NombreRol, Descripcion, Estado, FechaCreacion)
    VALUES (@NombreRol, @Descripcion, @Estado, GETDATE());

    SET @ID_Rol = SCOPE_IDENTITY();

    INSERT INTO RolPermiso (ID_Rol, ID_Permiso)
    SELECT DISTINCT @ID_Rol, CAST(value AS INT) FROM OPENJSON(@Permisos);

    COMMIT;

    SELECT @ID_Rol AS Value;
END
GO

CREATE OR ALTER PROCEDURE sp_Rol_Actualizar
    @ID_Rol      INT,
    @NombreRol   NVARCHAR(50),
    @Descripcion NVARCHAR(255) = NULL,
    @Permisos    NVARCHAR(MAX) = N'[]',
    @Estado BIT = NULL   -- NULL = no cambia el estado
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF NOT EXISTS (SELECT 1 FROM Rol WHERE ID_Rol = @ID_Rol)
        THROW 50404, N'El rol no existe', 1;

    EXEC sp_Rol_Validar @ID_Rol, @NombreRol, @Permisos;

    -- Protección: el Administrador siempre conserva SEGURIDAD (si no, nadie podría volver a asignar permisos)
    IF EXISTS (SELECT 1 FROM Rol WHERE ID_Rol = @ID_Rol AND NombreRol = N'Administrador')
       AND NOT EXISTS (SELECT 1 FROM OPENJSON(@Permisos) j
                         JOIN Permiso p ON p.ID_Permiso = TRY_CAST(j.value AS INT)
                        WHERE p.Codigo = N'SEGURIDAD')
        THROW 50001, N'No se puede quitar el permiso SEGURIDAD al rol Administrador', 1;

    IF @Estado = 0 AND EXISTS (SELECT 1 FROM Usuario WHERE ID_Rol = @ID_Rol AND Estado = 1)
        THROW 50001, N'No se puede desactivar un rol que tiene usuarios activos', 1;

    BEGIN TRANSACTION;

    UPDATE Rol
       SET NombreRol = @NombreRol,
           Descripcion = @Descripcion,
           FechaModificacion = GETDATE(),
           Estado = ISNULL(@Estado, Estado)
     WHERE ID_Rol = @ID_Rol;

    DELETE FROM RolPermiso WHERE ID_Rol = @ID_Rol;

    INSERT INTO RolPermiso (ID_Rol, ID_Permiso)
    SELECT DISTINCT @ID_Rol, CAST(value AS INT) FROM OPENJSON(@Permisos);

    COMMIT;
END
GO

CREATE OR ALTER PROCEDURE sp_Rol_CambiarEstado
    @ID_Rol INT,
    @Estado BIT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM Rol WHERE ID_Rol = @ID_Rol)
        THROW 50404, N'El rol no existe', 1;

    IF @Estado = 0 AND EXISTS (SELECT 1 FROM Usuario WHERE ID_Rol = @ID_Rol AND Estado = 1)
        THROW 50001, N'No se puede desactivar un rol que tiene usuarios activos', 1;

    UPDATE Rol SET Estado = @Estado, FechaModificacion = GETDATE() WHERE ID_Rol = @ID_Rol;
END
GO

-- ---------- PERMISOS ----------

CREATE OR ALTER PROCEDURE sp_Permiso_Listar
    @SoloActivos BIT = 0
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ID_Permiso, Codigo, Nombre, Descripcion, Estado
      FROM Permiso
     WHERE @SoloActivos = 0 OR Estado = 1
     ORDER BY Codigo;
END
GO

CREATE OR ALTER PROCEDURE sp_Permiso_Obtener
    @ID_Permiso INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ID_Permiso, Codigo, Nombre, Descripcion, Estado FROM Permiso WHERE ID_Permiso = @ID_Permiso;
END
GO

CREATE OR ALTER PROCEDURE sp_Permiso_Insertar
    @Codigo      NVARCHAR(50),
    @Nombre      NVARCHAR(100),
    @Descripcion NVARCHAR(255) = NULL,
    @Estado BIT = 1   -- 1 = activo, 0 = inactivo
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @ID INT;

    IF EXISTS (SELECT 1 FROM Permiso WHERE Codigo = UPPER(@Codigo))
        THROW 50001, N'Ya existe un permiso con ese código', 1;

    INSERT INTO Permiso (Codigo, Nombre, Descripcion, Estado)
    VALUES (UPPER(@Codigo), @Nombre, @Descripcion, @Estado);

    SET @ID = SCOPE_IDENTITY();
    EXEC sp_Permiso_Obtener @ID;
END
GO

CREATE OR ALTER PROCEDURE sp_Permiso_Actualizar
    @ID_Permiso  INT,
    @Codigo      NVARCHAR(50),
    @Nombre      NVARCHAR(100),
    @Descripcion NVARCHAR(255) = NULL,
    @Estado      BIT = 1
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM Permiso WHERE ID_Permiso = @ID_Permiso)
        THROW 50404, N'El permiso no existe', 1;

    IF EXISTS (SELECT 1 FROM Permiso WHERE Codigo = UPPER(@Codigo) AND ID_Permiso <> @ID_Permiso)
        THROW 50001, N'Ya existe un permiso con ese código', 1;

    UPDATE Permiso
       SET Codigo = UPPER(@Codigo), Nombre = @Nombre, Descripcion = @Descripcion, Estado = @Estado
     WHERE ID_Permiso = @ID_Permiso;
END
GO

CREATE OR ALTER PROCEDURE sp_Permiso_Eliminar
    @ID_Permiso INT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM Permiso WHERE ID_Permiso = @ID_Permiso)
        THROW 50404, N'El permiso no existe', 1;

    UPDATE Permiso SET Estado = 0 WHERE ID_Permiso = @ID_Permiso;
END
GO
