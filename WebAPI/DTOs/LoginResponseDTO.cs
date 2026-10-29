namespace WebAPI.DTOs
{
    // El frontend guarda esta información para armar el menú según los permisos del usuario
    public class LoginResponseDTO
    {
        public int ID_Usuario { get; set; }

        public string NombreCompleto { get; set; } = string.Empty;

        public string CorreoElectronico { get; set; } = string.Empty;

        public int ID_Rol { get; set; }

        public string NombreRol { get; set; } = string.Empty;

        public List<string> Permisos { get; set; } = new();

        public List<AlmacenSimpleDTO> Almacenes { get; set; } = new();
    }
}
