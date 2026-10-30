using System.ComponentModel.DataAnnotations;

namespace WebAPI.DTOs
{
    public class ProductoRequestDTO
    {
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(50, ErrorMessage = "El campo {0} solo permite 50 caracteres")]
        public string CodigoProducto { get; set; } = string.Empty;

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(150, ErrorMessage = "El campo {0} solo permite 150 caracteres")]
        public string NombreProducto { get; set; } = string.Empty;

        [StringLength(255)]
        public string? Descripcion { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "El campo {0} es requerido")]
        public int ID_Categoria { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "El campo {0} es requerido")]
        public int ID_UnidadMedida { get; set; }

        // Costo de compra. Cada compra lo actualiza con el costo pagado al proveedor
        [Range(0, 999999999, ErrorMessage = "El costo de compra no puede ser negativo")]
        public decimal Costo { get; set; }

        // Precio de venta
        [Range(0, 999999999, ErrorMessage = "El precio de venta no puede ser negativo")]
        public decimal Precio { get; set; }

        // true = activo, false = inactivo. Al crear, si no viene se toma como activo; al editar, si no viene no se cambia
        public bool? Estado { get; set; }
    }
}
