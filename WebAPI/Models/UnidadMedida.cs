using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Models
{
    [Index(nameof(Abreviatura), IsUnique = true)]
    public class UnidadMedida
    {
        [Key]
        public int ID_UnidadMedida { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(50)]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(10)]
        public string Abreviatura { get; set; } = string.Empty;

        public bool Estado { get; set; } = true;
    }
}
