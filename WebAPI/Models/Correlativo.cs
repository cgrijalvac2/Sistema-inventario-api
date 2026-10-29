using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Models
{
    // Guarda el último número usado de cada documento: VEN-000001, COM-000001, OC-000001...
    [Index(nameof(Clave), IsUnique = true)]
    public class Correlativo
    {
        [Key]
        public int ID_Correlativo { get; set; }

        // null para documentos que no son transacciones (órdenes de compra y transferencias)
        public int? ID_TipoTransaccion { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(30)]
        public string Clave { get; set; } = string.Empty;

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(10)]
        public string Prefijo { get; set; } = string.Empty;

        public int UltimoNumero { get; set; }

        public bool Estado { get; set; } = true;

        [ForeignKey(nameof(ID_TipoTransaccion))]
        public TipoTransaccion? TipoTransaccion { get; set; }
    }
}
