using System.ComponentModel.DataAnnotations;

namespace WebAPI.DTOs
{
    // Un registro de la tabla intermedia RolPermiso
    public class RolPermisoRequestDTO
    {
        [Range(1, int.MaxValue, ErrorMessage = "El campo {0} es requerido")]
        public int ID_Rol { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "El campo {0} es requerido")]
        public int ID_Permiso { get; set; }
    }
}
