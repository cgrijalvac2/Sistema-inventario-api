using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI.DTOs
{
    public class TransferenciaResponseDTO
    {
        public int ID_Transferencia { get; set; }

        public string NumeroTransferencia { get; set; } = string.Empty;

        public int ID_AlmacenOrigen { get; set; }

        public string AlmacenOrigen { get; set; } = string.Empty;

        public int ID_AlmacenDestino { get; set; }

        public string AlmacenDestino { get; set; } = string.Empty;

        public string UsuarioEnvia { get; set; } = string.Empty;

        public DateTime FechaEnvio { get; set; }

        public string? UsuarioRecibe { get; set; }

        public DateTime? FechaRecepcion { get; set; }

        public string? MotivoRechazo { get; set; }

        public string Estado { get; set; } = string.Empty;

        public string? Observaciones { get; set; }

        // Se llena con un segundo SP (el detalle no viene en el mismo SELECT)
        [NotMapped]
        public List<TransferenciaDetalleResponseDTO> Detalles { get; set; } = new();
    }

    public class TransferenciaDetalleResponseDTO
    {
        public int ID_DetalleTransferencia { get; set; }

        public int ID_Producto { get; set; }

        public string CodigoProducto { get; set; } = string.Empty;

        public string NombreProducto { get; set; } = string.Empty;

        public int Cantidad { get; set; }
    }
}
