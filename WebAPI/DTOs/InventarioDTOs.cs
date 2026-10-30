using System.ComponentModel.DataAnnotations;

namespace WebAPI.DTOs
{
    // Una fila del listado de existencias
    public class ExistenciaResponseDTO
    {
        public int ID_Almacen { get; set; }

        public string Almacen { get; set; } = string.Empty;

        public int ID_Producto { get; set; }

        public string CodigoProducto { get; set; } = string.Empty;

        public string NombreProducto { get; set; } = string.Empty;

        public string Categoria { get; set; } = string.Empty;

        public string UnidadMedida { get; set; } = string.Empty;

        public int StockActual { get; set; }

        public int StockMinimo { get; set; }

        public int StockMaximo { get; set; }

        public decimal Costo { get; set; }

        // Precio de venta del producto
        public decimal Precio { get; set; }

        // StockActual * Costo
        public decimal ValorInventario { get; set; }

        // BAJO, NORMAL o EXCESO
        public string Alerta { get; set; } = string.Empty;
    }

    // Una fila del kardex / registro de movimientos de entrada y salida
    public class MovimientoResponseDTO
    {
        public long ID_Movimiento { get; set; }

        public DateTime FechaRegistro { get; set; }

        public string TipoMovimiento { get; set; } = string.Empty;

        public string Concepto { get; set; } = string.Empty;

        public string Documento { get; set; } = string.Empty;

        public int ID_Almacen { get; set; }

        public string Almacen { get; set; } = string.Empty;

        public int ID_Producto { get; set; }

        public string CodigoProducto { get; set; } = string.Empty;

        public string NombreProducto { get; set; } = string.Empty;

        public int Entrada { get; set; }

        public int Salida { get; set; }

        public decimal CostoUnitario { get; set; }

        public int SaldoAnterior_Almacen { get; set; }

        public int SaldoActual_Almacen { get; set; }

        public int SaldoActual_Producto { get; set; }

        public string Usuario { get; set; } = string.Empty;
    }

    public class LimitesStockRequestDTO
    {
        [Range(0, int.MaxValue, ErrorMessage = "El campo {0} no puede ser negativo")]
        public int StockMinimo { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "El campo {0} no puede ser negativo")]
        public int StockMaximo { get; set; }
    }
}
