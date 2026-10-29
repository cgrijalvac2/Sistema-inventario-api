using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Models
{
    [Index(nameof(CorreoElectronico), IsUnique = true)]
    public class Usuario
    {
        [Key]
        public int ID_Usuario { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(150)]
        public string NombreCompleto { get; set; } = string.Empty;

        [EmailAddress]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(100)]
        public string CorreoElectronico { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string PasswordHash { get; set; } = string.Empty;

        public int ID_Rol { get; set; }

        public bool Estado { get; set; } = true;

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public DateTime? UltimoAcceso { get; set; }

        [ForeignKey(nameof(ID_Rol))]
        public Rol? Rol { get; set; }

        [JsonIgnore]
        public ICollection<UsuarioAlmacen> UsuarioAlmacenes { get; set; } = new List<UsuarioAlmacen>();
    }
}
