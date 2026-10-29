using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace WebAPI.Models
{
    public class TransaccionDetalle
    {
        [Key]
        public int ID_Detalle { get; set; }

        public int ID_Transaccion { get; set; }

        public int ID_Producto { get; set; }

        // Solo en ajustes
        public int? ID_Motivo { get; set; }

        public int Cantidad { get; set; }

        // Precio de venta (ventas) o costo de compra (compras)
        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecioUnitario { get; set; }

        // Costo del producto al momento del movimiento: sirve para valorizar el kardex y calcular la ganancia
        [Column(TypeName = "decimal(18,2)")]
        public decimal CostoUnitario { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Descuento { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(ID_Transaccion))]
        public Transaccion? Transaccion { get; set; }

        [ForeignKey(nameof(ID_Producto))]
        public Producto? Producto { get; set; }

        [ForeignKey(nameof(ID_Motivo))]
        public MotivoAjuste? MotivoAjuste { get; set; }
    }
}
