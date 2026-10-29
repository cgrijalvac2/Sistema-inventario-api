namespace WebAPI.DTOs
{
    // Fila de sp_Dashboard_Indicadores
    public class IndicadoresDTO
    {
        public decimal VentasHoy { get; set; }

        public decimal VentasMes { get; set; }

        public int CantidadVentasMes { get; set; }

        public decimal ComprasMes { get; set; }

        public int ProductosActivos { get; set; }

        public int ProductosStockBajo { get; set; }

        public decimal ValorInventario { get; set; }

        public int TransferenciasPorRecibir { get; set; }

        public int OrdenesPendientes { get; set; }
    }

    // Fila de sp_Dashboard_VentasPorDia
    public class VentaDiaDTO
    {
        public DateTime Fecha { get; set; }

        public decimal Total { get; set; }
    }

    // Fila de sp_Dashboard_TopProductos
    public class TopProductoDTO
    {
        public int ID_Producto { get; set; }

        public string CodigoProducto { get; set; } = string.Empty;

        public string NombreProducto { get; set; } = string.Empty;

        public int Cantidad { get; set; }

        public decimal Total { get; set; }
    }

    // Fila de sp_Inventario_Resumen
    public class ResumenAlmacenDTO
    {
        public int ID_Almacen { get; set; }

        public string Almacen { get; set; } = string.Empty;

        public int Productos { get; set; }

        public int Unidades { get; set; }

        public decimal ValorInventario { get; set; }
    }
}
