using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI.DTOs
{
    public class OrdenCompraResponseDTO
    {
        public int ID_OrdenCompra { get; set; }

        public string NumeroOrden { get; set; } = string.Empty;

        public int ID_Proveedor { get; set; }

        public string Proveedor { get; set; } = string.Empty;

        public int ID_Almacen { get; set; }

        public string Almacen { get; set; } = string.Empty;

        public int ID_Usuario { get; set; }

        public string Usuario { get; set; } = string.Empty;

        public DateTime FechaOrden { get; set; }

        public DateTime? FechaEsperada { get; set; }

        public string Estado { get; set; } = string.Empty;

        public decimal Subtotal { get; set; }

        public decimal Impuesto { get; set; }

        public decimal Total { get; set; }

        public string? Observaciones { get; set; }

        // Se llena con un segundo SP (el detalle no viene en el mismo SELECT)
        [NotMapped]
        public List<OrdenCompraDetalleResponseDTO> Detalles { get; set; } = new();
    }

    public class OrdenCompraDetalleResponseDTO
    {
        public int ID_DetalleOC { get; set; }

        public int ID_Producto { get; set; }

        public string CodigoProducto { get; set; } = string.Empty;

        public string NombreProducto { get; set; } = string.Empty;

        public int CantidadPedida { get; set; }

        public int CantidadRecibida { get; set; }

        public int CantidadPendiente { get; set; }

        public decimal CostoUnitario { get; set; }

        public decimal Descuento { get; set; }

        public decimal Total { get; set; }
    }
}
