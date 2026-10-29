using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Models
{
    // Kardex: una fila por cada entrada o salida de inventario. Nunca se modifica ni se borra
    [Index(nameof(ID_Producto), nameof(ID_Almacen), nameof(FechaRegistro))]
    public class HistorialMovimiento
    {
        [Key]
        public long ID_Movimiento { get; set; }

        public int ID_Producto { get; set; }

        public int ID_Almacen { get; set; }

        public int ID_Usuario { get; set; }

        // Origen del movimiento: una línea de transacción o una línea de transferencia
        public int? ID_Detalle { get; set; }

        public int? ID_DetalleTransferencia { get; set; }

        // ENTRADA o SALIDA
        [Required]
        [StringLength(10)]
        public string TipoMovimiento { get; set; } = string.Empty;

        // Ej: VENTA, COMPRA, TRANSFERENCIA, ANULACION VENTA
        [Required]
        [StringLength(50)]
        public string Concepto { get; set; } = string.Empty;

        // Número del documento que lo generó (VEN-000001, TRF-000003...)
        [Required]
        [StringLength(30)]
        public string Documento { get; set; } = string.Empty;

        public int Cantidad { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal CostoUnitario { get; set; }

        public int SaldoAnterior_Almacen { get; set; }

        public int SaldoActual_Almacen { get; set; }

        // Existencia total del producto sumando todos los almacenes
        public int SaldoActual_Producto { get; set; }

        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        [ForeignKey(nameof(ID_Producto))]
        public Producto? Producto { get; set; }

        [ForeignKey(nameof(ID_Almacen))]
        public Almacen? Almacen { get; set; }

        [ForeignKey(nameof(ID_Usuario))]
        public Usuario? Usuario { get; set; }

        [ForeignKey(nameof(ID_Detalle))]
        public TransaccionDetalle? TransaccionDetalle { get; set; }

        [ForeignKey(nameof(ID_DetalleTransferencia))]
        public TransferenciaDetalle? TransferenciaDetalle { get; set; }
    }
}
