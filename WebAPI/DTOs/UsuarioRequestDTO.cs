using System.ComponentModel.DataAnnotations;

namespace WebAPI.DTOs
{
    public class UsuarioRequestDTO
    {
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(150, ErrorMessage = "El campo {0} solo permite 150 caracteres")]
        public string NombreCompleto { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "El formato del campo {0} es inválido")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(100, ErrorMessage = "El campo {0} solo permite 100 caracteres")]
        public string CorreoElectronico { get; set; } = string.Empty;

        // Obligatorio al crear. Al modificar se puede dejar vacío para no cambiarlo
        [MinLength(8, ErrorMessage = "El campo {0} requiere mínimo 8 caracteres")]
        public string? Password { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "El campo {0} es requerido")]
        public int ID_Rol { get; set; }

        // Almacenes en los que podrá trabajar
        public List<int> Almacenes { get; set; } = new();
    }
}
