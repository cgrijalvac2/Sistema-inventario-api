using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models
{
    public class MotivoAjuste
    {
        [Key]
        public int ID_Motivo { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(100)]
        public string Descripcion { get; set; } = string.Empty;

        public bool Estado { get; set; } = true;
    }
}
