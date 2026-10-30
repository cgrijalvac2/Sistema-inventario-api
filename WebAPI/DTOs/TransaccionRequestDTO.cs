using System.ComponentModel.DataAnnotations;

namespace WebAPI.DTOs
{
    // Un solo DTO sirve para compras, ventas, notas de crédito, notas de débito y ajustes
    public class TransaccionRequestDTO
    {
        // COMPRA, VENTA, NC_VENTA, NC_COMPRA, ND_VENTA, ND_COMPRA, AJUSTE_ENTRADA, AJUSTE_SALIDA
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string CodigoTipoTransaccion { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "El campo {0} es requerido")]
        public int ID_Usuario { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "El campo {0} es requerido")]
        public int ID_Almacen { get; set; }

        // Requerido en ventas y notas a clientes
        public int? ID_Cliente { get; set; }

        // Requerido en compras y notas de proveedor
        public int? ID_Proveedor { get; set; }

        // Opcional: compra que recibe una orden de compra
        public int? ID_OrdenCompra { get; set; }

        // Requerido en notas de crédito y débito: la venta o compra que se corrige
        public int? ID_TransaccionOrigen { get; set; }

        [StringLength(100)]
        public string? Referencia { get; set; }

        [StringLength(255)]
        public string? Observaciones { get; set; }

        // Solo notas de crédito: true = el producto regresa / sale del inventario, false = no mueve inventario
        // (ej: descuento posterior o producto dañado que no vuelve). Si no viene se toma como true
        public bool? DevuelveInventario { get; set; }

        [MinLength(1, ErrorMessage = "El documento debe tener al menos un producto")]
        public List<TransaccionDetalleRequestDTO> Detalles { get; set; } = new();
    }

    public class TransaccionDetalleRequestDTO
    {
        [Range(1, int.MaxValue, ErrorMessage = "El campo {0} es requerido")]
        public int ID_Producto { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a cero")]
        public int Cantidad { get; set; }

        // Si no se envía: en ventas se usa el precio del producto y en compras/ajustes su costo
        [Range(0, 999999999, ErrorMessage = "El campo {0} no puede ser negativo")]
        public decimal? PrecioUnitario { get; set; }

        [Range(0, 999999999, ErrorMessage = "El campo {0} no puede ser negativo")]
        public decimal Descuento { get; set; }

        // Requerido en ajustes
        public int? ID_Motivo { get; set; }
    }
}
