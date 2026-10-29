using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Models
{
    // Un contacto solo puede registrarse una vez como cliente (relación 1 a 1)
    [Index(nameof(ID_Contacto), IsUnique = true)]
    public class Cliente
    {
        [Key]
        public int ID_Cliente { get; set; }

        public int ID_Contacto { get; set; }

        public bool Estado { get; set; } = true;

        [ForeignKey(nameof(ID_Contacto))]
        public DirectorioContacto? Contacto { get; set; }
    }
}
