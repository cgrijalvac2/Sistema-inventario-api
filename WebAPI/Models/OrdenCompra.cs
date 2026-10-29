using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Models
{
    // Pedido al proveedor. No mueve inventario: el inventario entra cuando se registra la COMPRA
    [Index(nameof(NumeroOrden), IsUnique = true)]
    public class OrdenCompra
    {
        [Key]
        public int ID_OrdenCompra { get; set; }

        [Required]
        [StringLength(30)]
        public string NumeroOrden { get; set; } = string.Empty;

        public int ID_Proveedor { get; set; }

        // Almacén donde se va a recibir la mercadería
        public int ID_Almacen { get; set; }

        public int ID_Usuario { get; set; }

        public DateTime FechaOrden { get; set; } = DateTime.Now;

        [Column(TypeName = "date")]
        public DateTime? FechaEsperada { get; set; }

        // PENDIENTE, PARCIAL, COMPLETADA, CANCELADA
        [Required]
        [StringLength(20)]
        public string Estado { get; set; } = "PENDIENTE";

        [Column(TypeName = "decimal(18,2)")]
        public decimal Subtotal { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Impuesto { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }

        [StringLength(255)]
        public string? Observaciones { get; set; }

        public DateTime? FechaModificacion { get; set; }

        [ForeignKey(nameof(ID_Proveedor))]
        public Proveedor? Proveedor { get; set; }

        [ForeignKey(nameof(ID_Almacen))]
        public Almacen? Almacen { get; set; }

        [ForeignKey(nameof(ID_Usuario))]
        public Usuario? Usuario { get; set; }

        public ICollection<OrdenCompraDetalle> Detalles { get; set; } = new List<OrdenCompraDetalle>();
    }
}
