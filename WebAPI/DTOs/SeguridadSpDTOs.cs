namespace WebAPI.DTOs
{
    // Fila de sp_Auth_ObtenerUsuario. Es interna: nunca se devuelve al frontend porque trae el hash
    public class AuthUsuarioDTO
    {
        public int ID_Usuario { get; set; }

        public string NombreCompleto { get; set; } = string.Empty;

        public string CorreoElectronico { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public int ID_Rol { get; set; }

        public string NombreRol { get; set; } = string.Empty;

        public bool Estado { get; set; }

        public bool RolEstado { get; set; }
    }

    // Fila de sp_Usuario_Almacenes: almacén asignado a un usuario
    public class UsuarioAlmacenDTO : AlmacenSimpleDTO
    {
        public int ID_Usuario { get; set; }
    }

    // Fila de sp_Rol_Permisos: permiso asignado a un rol
    public class RolPermisoDTO : PermisoSimpleDTO
    {
        public int ID_Rol { get; set; }
    }
}
