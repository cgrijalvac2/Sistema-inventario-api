using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Models
{
    // Existencia de cada producto en cada almacén. El stock solo lo modifica InventarioHelper
    [PrimaryKey(nameof(ID_Almacen), nameof(ID_Producto))]
    public class InventarioAlmacen
    {
        public int ID_Almacen { get; set; }

        public int ID_Producto { get; set; }

        public int StockActual { get; set; }

        public int StockMinimo { get; set; }

        public int StockMaximo { get; set; }

        public DateTime? FechaUltimoMovimiento { get; set; }

        [ForeignKey(nameof(ID_Almacen))]
        public Almacen? Almacen { get; set; }

        [ForeignKey(nameof(ID_Producto))]
        public Producto? Producto { get; set; }
    }
}
