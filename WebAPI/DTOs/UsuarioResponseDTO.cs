using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI.DTOs
{
    // Nunca se devuelve el PasswordHash
    public class UsuarioResponseDTO
    {
        public int ID_Usuario { get; set; }

        public string NombreCompleto { get; set; } = string.Empty;

        public string CorreoElectronico { get; set; } = string.Empty;

        public int ID_Rol { get; set; }

        public string NombreRol { get; set; } = string.Empty;

        public bool Estado { get; set; }

        public DateTime FechaCreacion { get; set; }

        public DateTime? UltimoAcceso { get; set; }

        // Se llena con un segundo SP (el detalle no viene en el mismo SELECT)
        [NotMapped]
        public List<AlmacenSimpleDTO> Almacenes { get; set; } = new();
    }
}
