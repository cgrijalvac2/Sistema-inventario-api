using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Models
{
    [Index(nameof(NombreRol), IsUnique = true)]
    public class Rol
    {
        [Key]
        public int ID_Rol { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(50)]
        public string NombreRol { get; set; } = string.Empty;

        [StringLength(255)]
        public string? Descripcion { get; set; }

        public bool Estado { get; set; } = true;

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public DateTime? FechaModificacion { get; set; }

        [JsonIgnore]
        public ICollection<RolPermiso> RolPermisos { get; set; } = new List<RolPermiso>();
    }
}
