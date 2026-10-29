using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI.DTOs
{
    public class RolResponseDTO
    {
        public int ID_Rol { get; set; }

        public string NombreRol { get; set; } = string.Empty;

        public string? Descripcion { get; set; }

        public bool Estado { get; set; }

        // Se llena con un segundo SP (el detalle no viene en el mismo SELECT)
        [NotMapped]
        public List<PermisoSimpleDTO> Permisos { get; set; } = new();
    }

    public class PermisoSimpleDTO
    {
        public int ID_Permiso { get; set; }

        public string Codigo { get; set; } = string.Empty;

        public string Nombre { get; set; } = string.Empty;
    }
}
