using System.ComponentModel.DataAnnotations;

namespace WebAPI.DTOs
{
    public class LoginRequestDTO
    {
        [EmailAddress(ErrorMessage = "El formato del campo {0} es inválido")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string CorreoElectronico { get; set; } = string.Empty;

        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string Password { get; set; } = string.Empty;
    }
}
