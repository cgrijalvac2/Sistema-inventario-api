using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Models
{
    [Index(nameof(NombreCategoria), IsUnique = true)]
    public class Categoria
    {
        [Key]
        public int ID_Categoria { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(100)]
        public string NombreCategoria { get; set; } = string.Empty;

        [StringLength(255)]
        public string? DescripcionCategoria { get; set; }

        public bool Estado { get; set; } = true;
    }
}
