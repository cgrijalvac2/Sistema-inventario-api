using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI.DTOs
{
    public class TransaccionResponseDTO
    {
        public int ID_Transaccion { get; set; }

        public string NumeroDocumento { get; set; } = string.Empty;

        public string CodigoTipoTransaccion { get; set; } = string.Empty;

        public string TipoTransaccion { get; set; } = string.Empty;

        public DateTime Fecha { get; set; }

        public int ID_Almacen { get; set; }

        public string Almacen { get; set; } = string.Empty;

        public int ID_Usuario { get; set; }

        public string Usuario { get; set; } = string.Empty;

        public int? ID_Cliente { get; set; }

        public string? Cliente { get; set; }

        public int? ID_Proveedor { get; set; }

        public string? Proveedor { get; set; }

        public int? ID_OrdenCompra { get; set; }

        public string? NumeroOrden { get; set; }

        public int? ID_TransaccionOrigen { get; set; }

        public string? DocumentoOrigen { get; set; }

        public string? Referencia { get; set; }

        public decimal Subtotal { get; set; }

        public decimal Descuento { get; set; }

        public decimal Impuesto { get; set; }

        public decimal Total { get; set; }

        public string Estado { get; set; } = string.Empty;

        public string? Observaciones { get; set; }

        public DateTime? FechaAnulacion { get; set; }

        // Se llena con un segundo SP (el detalle no viene en el mismo SELECT)
        [NotMapped]
        public List<TransaccionDetalleResponseDTO> Detalles { get; set; } = new();
    }

    public class TransaccionDetalleResponseDTO
    {
        public int ID_Detalle { get; set; }

        public int ID_Producto { get; set; }

        public string CodigoProducto { get; set; } = string.Empty;

        public string NombreProducto { get; set; } = string.Empty;

        public int? ID_Motivo { get; set; }

        public string? Motivo { get; set; }

        public int Cantidad { get; set; }

        public decimal PrecioUnitario { get; set; }

        public decimal CostoUnitario { get; set; }

        public decimal Descuento { get; set; }

        public decimal Total { get; set; }
    }
}
