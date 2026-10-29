using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Models
{
    // Un contacto solo puede registrarse una vez como proveedor (relación 1 a 1)
    [Index(nameof(ID_Contacto), IsUnique = true)]
    public class Proveedor
    {
        [Key]
        public int ID_Proveedor { get; set; }

        public int ID_Contacto { get; set; }

        [Range(0, 365)]
        public int DiasCredito { get; set; }

        public bool Estado { get; set; } = true;

        [ForeignKey(nameof(ID_Contacto))]
        public DirectorioContacto? Contacto { get; set; }
    }
}
