using System.ComponentModel.DataAnnotations;

namespace WebAPI.DTOs
{
    // Productos que se agregan a un almacén (tabla InventarioAlmacen)
    public class AsignarProductosRequestDTO
    {
        [Range(1, int.MaxValue, ErrorMessage = "El campo {0} es requerido")]
        public int ID_Almacen { get; set; }

        // Usuario que registra el ajuste de la existencia inicial
        [Range(1, int.MaxValue, ErrorMessage = "El campo {0} es requerido")]
        public int ID_Usuario { get; set; }

        [MinLength(1, ErrorMessage = "Seleccione al menos un producto")]
        public List<ProductoAlmacenRequestDTO> Productos { get; set; } = new();
    }

    public class ProductoAlmacenRequestDTO
    {
        [Range(1, int.MaxValue, ErrorMessage = "El campo {0} es requerido")]
        public int ID_Producto { get; set; }

        // Existencia actual: si es mayor a 0 se registra un ajuste de entrada (Inventario inicial)
        [Range(0, int.MaxValue, ErrorMessage = "La cantidad no puede ser negativa")]
        public int Cantidad { get; set; }

        // Costo de esa existencia; vacío = costo actual del producto
        [Range(0, 999999999, ErrorMessage = "El costo no puede ser negativo")]
        public decimal? CostoUnitario { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "El campo {0} no puede ser negativo")]
        public int StockMinimo { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "El campo {0} no puede ser negativo")]
        public int StockMaximo { get; set; }
    }

    // Editar un producto ya asignado: límites y, opcionalmente, la existencia contada físicamente
    public class ActualizarProductoAlmacenRequestDTO
    {
        [Range(1, int.MaxValue, ErrorMessage = "El campo {0} es requerido")]
        public int ID_Usuario { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "El campo {0} no puede ser negativo")]
        public int StockMinimo { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "El campo {0} no puede ser negativo")]
        public int StockMaximo { get; set; }

        // null = no cambiar la existencia. Si cambia, se registra un ajuste por la diferencia
        [Range(0, int.MaxValue, ErrorMessage = "La cantidad no puede ser negativa")]
        public int? CantidadActual { get; set; }
    }
}
