using System.ComponentModel.DataAnnotations;

namespace WebAPI.DTOs
{
    // Se usa para crear o modificar clientes. Los datos se guardan en DirectorioContacto
    public class ClienteRequestDTO
    {
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(150, ErrorMessage = "El campo {0} solo permite 150 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(20, ErrorMessage = "El campo {0} solo permite 20 caracteres")]
        public string NIT { get; set; } = "CF";

        [StringLength(255)]
        public string? Direccion { get; set; }

        [StringLength(50)]
        public string? Telefono { get; set; }

        [EmailAddress(ErrorMessage = "El formato del campo {0} es inválido")]
        [StringLength(100)]
        public string? CorreoElectronico { get; set; }
    }

    // El proveedor lleva los mismos datos que el cliente más los días de crédito
    public class ProveedorRequestDTO : ClienteRequestDTO
    {
        [Range(0, 365, ErrorMessage = "El campo {0} debe estar entre 0 y 365")]
        public int DiasCredito { get; set; }
    }
}
