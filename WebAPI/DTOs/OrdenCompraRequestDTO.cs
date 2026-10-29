using System.ComponentModel.DataAnnotations;

namespace WebAPI.DTOs
{
    public class OrdenCompraRequestDTO
    {
        [Range(1, int.MaxValue, ErrorMessage = "El campo {0} es requerido")]
        public int ID_Proveedor { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "El campo {0} es requerido")]
        public int ID_Almacen { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "El campo {0} es requerido")]
        public int ID_Usuario { get; set; }

        public DateTime? FechaEsperada { get; set; }

        [StringLength(255)]
        public string? Observaciones { get; set; }

        [MinLength(1, ErrorMessage = "La orden debe tener al menos un producto")]
        public List<OrdenCompraDetalleRequestDTO> Detalles { get; set; } = new();
    }

    public class OrdenCompraDetalleRequestDTO
    {
        [Range(1, int.MaxValue, ErrorMessage = "El campo {0} es requerido")]
        public int ID_Producto { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a cero")]
        public int Cantidad { get; set; }

        [Range(0, 999999999, ErrorMessage = "El campo {0} no puede ser negativo")]
        public decimal CostoUnitario { get; set; }

        [Range(0, 999999999, ErrorMessage = "El campo {0} no puede ser negativo")]
        public decimal Descuento { get; set; }
    }
}
