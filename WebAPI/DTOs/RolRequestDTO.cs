using System.ComponentModel.DataAnnotations;

namespace WebAPI.DTOs
{
    public class RolRequestDTO
    {
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(50, ErrorMessage = "El campo {0} solo permite 50 caracteres")]
        public string NombreRol { get; set; } = string.Empty;

        [StringLength(255)]
        public string? Descripcion { get; set; }

        // ID de los permisos que tendrá el rol
        public List<int> Permisos { get; set; } = new();
    }
}
