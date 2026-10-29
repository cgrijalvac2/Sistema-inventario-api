using System.ComponentModel.DataAnnotations;

namespace WebAPI.DTOs
{
    public class TransferenciaRequestDTO
    {
        [Range(1, int.MaxValue, ErrorMessage = "El campo {0} es requerido")]
        public int ID_AlmacenOrigen { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "El campo {0} es requerido")]
        public int ID_AlmacenDestino { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "El campo {0} es requerido")]
        public int ID_Usuario { get; set; }

        [StringLength(255)]
        public string? Observaciones { get; set; }

        [MinLength(1, ErrorMessage = "La transferencia debe tener al menos un producto")]
        public List<TransferenciaDetalleRequestDTO> Detalles { get; set; } = new();
    }

    public class TransferenciaDetalleRequestDTO
    {
        [Range(1, int.MaxValue, ErrorMessage = "El campo {0} es requerido")]
        public int ID_Producto { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a cero")]
        public int Cantidad { get; set; }
    }

    // Para recibir, rechazar o anular un documento
    public class AccionUsuarioRequestDTO
    {
        [Range(1, int.MaxValue, ErrorMessage = "El campo {0} es requerido")]
        public int ID_Usuario { get; set; }

        [StringLength(255)]
        public string? Motivo { get; set; }
    }
}
