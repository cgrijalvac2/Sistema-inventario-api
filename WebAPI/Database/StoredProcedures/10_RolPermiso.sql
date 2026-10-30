/* =====================================================================
   10 - ROL PERMISO (tabla intermedia RolPermiso)
   Define a qué permisos tiene acceso cada rol. La consulta se hace con sp_Rol_Permisos (02_Seguridad.sql).
   Los cambios se aplican a los usuarios la próxima vez que inicien sesión.
   ===================================================================== */

-- Da acceso a un permiso. Si ya lo tenía no hace nada
CREATE OR ALTER PROCEDURE sp_RolPermiso_Asignar
    @ID_Rol     INT,
    @ID_Permiso INT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM Rol WHERE ID_Rol = @ID_Rol AND Estado = 1)
        THROW 50001, N'El rol no existe o está inactivo', 1;

    IF NOT EXISTS (SELECT 1 FROM Permiso WHERE ID_Permiso = @ID_Permiso AND Estado = 1)
        THROW 50001, N'El permiso no existe o está inactivo', 1;

    IF NOT EXISTS (SELECT 1 FROM RolPermiso WHERE ID_Rol = @ID_Rol AND ID_Permiso = @ID_Permiso)
        INSERT INTO RolPermiso (ID_Rol, ID_Permiso) VALUES (@ID_Rol, @ID_Permiso);

    UPDATE Rol SET FechaModificacion = GETDATE() WHERE ID_Rol = @ID_Rol;
END
GO

-- Quita el acceso a un permiso
CREATE OR ALTER PROCEDURE sp_RolPermiso_Quitar
    @ID_Rol     INT,
    @ID_Permiso INT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM RolPermiso WHERE ID_Rol = @ID_Rol AND ID_Permiso = @ID_Permiso)
        THROW 50404, N'El rol no tiene ese permiso', 1;

    -- Protección: si el Administrador pierde SEGURIDAD, nadie podría volver a asignar permisos
    IF EXISTS (SELECT 1 FROM Rol r, Permiso p
                WHERE r.ID_Rol = @ID_Rol AND r.NombreRol = N'Administrador'
                  AND p.ID_Permiso = @ID_Permiso AND p.Codigo = N'SEGURIDAD')
        THROW 50001, N'No se puede quitar el permiso SEGURIDAD al rol Administrador', 1;

    DELETE FROM RolPermiso WHERE ID_Rol = @ID_Rol AND ID_Permiso = @ID_Permiso;

    UPDATE Rol SET FechaModificacion = GETDATE() WHERE ID_Rol = @ID_Rol;
END
GO
