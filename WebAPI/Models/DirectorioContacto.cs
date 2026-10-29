using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models
{
    // Datos comunes de clientes y proveedores (una misma empresa puede ser cliente y proveedor)
    public class DirectorioContacto
    {
        [Key]
        public int ID_Contacto { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(150)]
        public string Nombre { get; set; } = string.Empty;

        // No es único: muchos clientes pueden ser "CF" (consumidor final)
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(20)]
        public string NIT { get; set; } = "CF";

        [StringLength(255)]
        public string? Direccion { get; set; }

        [StringLength(50)]
        public string? Telefono { get; set; }

        [EmailAddress]
        [StringLength(100)]
        public string? CorreoElectronico { get; set; }

        public bool Estado { get; set; } = true;
    }
}
