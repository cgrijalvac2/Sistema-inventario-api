using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Models
{
    // Almacenes en los que puede trabajar cada usuario (antes "UsuarioTienda")
    [PrimaryKey(nameof(ID_Usuario), nameof(ID_Almacen))]
    public class UsuarioAlmacen
    {
        public int ID_Usuario { get; set; }

        public int ID_Almacen { get; set; }

        [ForeignKey(nameof(ID_Usuario))]
        public Usuario? Usuario { get; set; }

        [ForeignKey(nameof(ID_Almacen))]
        public Almacen? Almacen { get; set; }
    }
}
