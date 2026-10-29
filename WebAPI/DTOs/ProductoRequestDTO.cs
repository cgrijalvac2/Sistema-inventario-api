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

        // Costo inicial. Después lo recalculan automáticamente las compras
        [Range(0, 999999999, ErrorMessage = "El campo {0} no puede ser negativo")]
        public decimal Costo { get; set; }

        [Range(0, 999999999, ErrorMessage = "El campo {0} no puede ser negativo")]
        public decimal Precio { get; set; }
    }
}
