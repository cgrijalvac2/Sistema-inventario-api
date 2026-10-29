using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Models
{
    // Traslado de mercadería entre almacenes: ENVIADA -> RECIBIDA o RECHAZADA
    [Index(nameof(NumeroTransferencia), IsUnique = true)]
    public class Transferencia
    {
        [Key]
        public int ID_Transferencia { get; set; }

        [Required]
        [StringLength(30)]
        public string NumeroTransferencia { get; set; } = string.Empty;

        public int ID_AlmacenOrigen { get; set; }

        public int ID_AlmacenDestino { get; set; }

        public int ID_UsuarioEnvia { get; set; }

        public DateTime FechaEnvio { get; set; } = DateTime.Now;

        // Usuario del almacén destino que la recibió o la rechazó
        public int? ID_UsuarioRecibe { get; set; }

        public DateTime? FechaRecepcion { get; set; }

        [StringLength(255)]
        public string? MotivoRechazo { get; set; }

        // ENVIADA, RECIBIDA, RECHAZADA
        [Required]
        [StringLength(20)]
        public string Estado { get; set; } = "ENVIADA";

        [StringLength(255)]
        public string? Observaciones { get; set; }

        [ForeignKey(nameof(ID_AlmacenOrigen))]
        public Almacen? AlmacenOrigen { get; set; }

        [ForeignKey(nameof(ID_AlmacenDestino))]
        public Almacen? AlmacenDestino { get; set; }

        [ForeignKey(nameof(ID_UsuarioEnvia))]
        public Usuario? UsuarioEnvia { get; set; }

        [ForeignKey(nameof(ID_UsuarioRecibe))]
        public Usuario? UsuarioRecibe { get; set; }

        public ICollection<TransferenciaDetalle> Detalles { get; set; } = new List<TransferenciaDetalle>();
    }
}
