using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Models
{
    // Encabezado de compras, ventas, notas de crédito, notas de débito y ajustes
    [Index(nameof(NumeroDocumento), IsUnique = true)]
    public class Transaccion
    {
        [Key]
        public int ID_Transaccion { get; set; }

        [Required]
        [StringLength(30)]
        public string NumeroDocumento { get; set; } = string.Empty;

        public int ID_TipoTransaccion { get; set; }

        public int ID_Usuario { get; set; }

        public int ID_Almacen { get; set; }

        public int? ID_Cliente { get; set; }

        public int? ID_Proveedor { get; set; }

        // Compra que recibe una orden de compra
        public int? ID_OrdenCompra { get; set; }

        // Venta o compra que corrige una nota de crédito / débito
        public int? ID_TransaccionOrigen { get; set; }

        public DateTime Fecha { get; set; } = DateTime.Now;

        // Ej: número de factura del proveedor
        [StringLength(100)]
        public string? Referencia { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Subtotal { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Descuento { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Impuesto { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }

        // REGISTRADA o ANULADA. Los documentos no se editan ni se borran: se anulan
        [Required]
        [StringLength(20)]
        public string Estado { get; set; } = "REGISTRADA";

        [StringLength(255)]
        public string? Observaciones { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public int? ID_UsuarioAnula { get; set; }

        public DateTime? FechaAnulacion { get; set; }

        [ForeignKey(nameof(ID_TipoTransaccion))]
        public TipoTransaccion? TipoTransaccion { get; set; }

        [ForeignKey(nameof(ID_Usuario))]
        public Usuario? Usuario { get; set; }

        [ForeignKey(nameof(ID_Almacen))]
        public Almacen? Almacen { get; set; }

        [ForeignKey(nameof(ID_Cliente))]
        public Cliente? Cliente { get; set; }

        [ForeignKey(nameof(ID_Proveedor))]
        public Proveedor? Proveedor { get; set; }

        [ForeignKey(nameof(ID_OrdenCompra))]
        public OrdenCompra? OrdenCompra { get; set; }

        [ForeignKey(nameof(ID_TransaccionOrigen))]
        public Transaccion? TransaccionOrigen { get; set; }

        [ForeignKey(nameof(ID_UsuarioAnula))]
        public Usuario? UsuarioAnula { get; set; }

        public ICollection<TransaccionDetalle> Detalles { get; set; } = new List<TransaccionDetalle>();
    }
}
