using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Models
{
    [Index(nameof(Codigo), IsUnique = true)]
    public class Permiso
    {
        [Key]
        public int ID_Permiso { get; set; }

        // Ej: PRODUCTOS, VENTAS. El frontend lo usa para mostrar u ocultar opciones del menú
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(50)]
        public string Codigo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(255)]
        public string? Descripcion { get; set; }

        public bool Estado { get; set; } = true;
    }
}
