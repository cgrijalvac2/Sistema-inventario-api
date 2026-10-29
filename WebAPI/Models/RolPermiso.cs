using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Models
{
    // Tabla intermedia (muchos a muchos): la llave primaria es la combinación de los dos ID
    [PrimaryKey(nameof(ID_Rol), nameof(ID_Permiso))]
    public class RolPermiso
    {
        public int ID_Rol { get; set; }

        public int ID_Permiso { get; set; }

        [ForeignKey(nameof(ID_Rol))]
        public Rol? Rol { get; set; }

        [ForeignKey(nameof(ID_Permiso))]
        public Permiso? Permiso { get; set; }
    }
}
