using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Models
{
    [Index(nameof(Clave), nameof(ID_Almacen), IsUnique = true)]
    public class ParametroSistema
    {
        [Key]
        public int ID_Parametro { get; set; }

        // null = parámetro global; con valor = parámetro propio de ese almacén
        public int? ID_Almacen { get; set; }

        // Ej: IVA_PORCENTAJE, MONEDA, NOMBRE_EMPRESA
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(50)]
        public string Clave { get; set; } = string.Empty;

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(255)]
        public string Valor { get; set; } = string.Empty;

        [StringLength(255)]
        public string? Descripcion { get; set; }

        public bool Estado { get; set; } = true;

        public DateTime? FechaModificacion { get; set; }

        [ForeignKey(nameof(ID_Almacen))]
        public Almacen? Almacen { get; set; }
    }
}
