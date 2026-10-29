using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace WebAPI.Models
{
    public class OrdenCompraDetalle
    {
        [Key]
        public int ID_DetalleOC { get; set; }

        public int ID_OrdenCompra { get; set; }

        public int ID_Producto { get; set; }

        public int CantidadPedida { get; set; }

        // Se va sumando con cada compra que hace referencia a esta orden
        public int CantidadRecibida { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal CostoUnitario { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Descuento { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(ID_OrdenCompra))]
        public OrdenCompra? OrdenCompra { get; set; }

        [ForeignKey(nameof(ID_Producto))]
        public Producto? Producto { get; set; }
    }
}
