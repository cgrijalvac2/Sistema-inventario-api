namespace WebAPI.DTOs
{
    public class ProductoResponseDTO
    {
        public int ID_Producto { get; set; }

        public string CodigoProducto { get; set; } = string.Empty;

        public string NombreProducto { get; set; } = string.Empty;

        public string? Descripcion { get; set; }

        public int ID_Categoria { get; set; }

        public string NombreCategoria { get; set; } = string.Empty;

        public int ID_UnidadMedida { get; set; }

        public string UnidadMedida { get; set; } = string.Empty;

        public decimal Costo { get; set; }

        public decimal Precio { get; set; }

        // Suma de la existencia en todos los almacenes
        public int StockTotal { get; set; }

        public bool Estado { get; set; }
    }
}
