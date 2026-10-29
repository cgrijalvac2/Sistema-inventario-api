using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace WebAPI.Models
{
    public class TransferenciaDetalle
    {
        [Key]
        public int ID_DetalleTransferencia { get; set; }

        public int ID_Transferencia { get; set; }

        public int ID_Producto { get; set; }

        public int Cantidad { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(ID_Transferencia))]
        public Transferencia? Transferencia { get; set; }

        [ForeignKey(nameof(ID_Producto))]
        public Producto? Producto { get; set; }
    }
}
