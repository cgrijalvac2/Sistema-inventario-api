using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Models
{
    [Index(nameof(Codigo), IsUnique = true)]
    public class Almacen
    {
        [Key]
        public int ID_Almacen { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(20)]
        public string Codigo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(255)]
        public string? Direccion { get; set; }

        [StringLength(255)]
        public string? Descripcion { get; set; }

        public bool Estado { get; set; } = true;
    }
}
