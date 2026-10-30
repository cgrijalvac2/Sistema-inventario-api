using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Models
{
    [Index(nameof(CodigoProducto), IsUnique = true)]
    public class Producto
    {
        [Key]
        public int ID_Producto { get; set; }

        public int ID_Categoria { get; set; }

        public int ID_UnidadMedida { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(50)]
        public string CodigoProducto { get; set; } = string.Empty;

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(150)]
        public string NombreProducto { get; set; } = string.Empty;

        [StringLength(255)]
        public string? Descripcion { get; set; }

        // Costo de compra: se captura en el producto y cada compra lo actualiza con el último costo pagado
        [Column(TypeName = "decimal(18,2)")]
        public decimal Costo { get; set; }

        // Precio de venta sugerido
        [Column(TypeName = "decimal(18,2)")]
        public decimal Precio { get; set; }

        public bool Estado { get; set; } = true;

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        [ForeignKey(nameof(ID_Categoria))]
        public Categoria? Categoria { get; set; }

        [ForeignKey(nameof(ID_UnidadMedida))]
        public UnidadMedida? UnidadMedida { get; set; }
    }
}
