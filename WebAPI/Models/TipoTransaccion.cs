using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Models
{
    [Index(nameof(Codigo), IsUnique = true)]
    public class TipoTransaccion
    {
        [Key]
        public int ID_TipoTransaccion { get; set; }

        // COMPRA, VENTA, NC_VENTA, NC_COMPRA, ND_VENTA, ND_COMPRA, AJUSTE_ENTRADA, AJUSTE_SALIDA
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(20)]
        public string Codigo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(50)]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(255)]
        public string? Descripcion { get; set; }

        // +1 = entra al inventario, -1 = sale del inventario, 0 = no mueve inventario (notas de débito)
        [Range(-1, 1)]
        public int Factor_Kardex { get; set; }

        // CLIENTE, PROVEEDOR o NINGUNO: a quién va dirigido el documento
        [Required]
        [StringLength(10)]
        public string TipoTercero { get; set; } = "NINGUNO";

        // true en notas de crédito y débito: deben indicar la venta o compra que corrigen
        public bool RequiereDocumentoOrigen { get; set; }

        // true en ajustes: cada línea debe llevar un motivo
        public bool RequiereMotivo { get; set; }

        public bool Estado { get; set; } = true;
    }
}
