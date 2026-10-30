using Microsoft.AspNetCore.Mvc;
using WebAPI.Contexts;
using WebAPI.DTOs;
using WebAPI.Helpers;
using static WebAPI.Helpers.SpHelper;

namespace WebAPI.Controllers
{
    // Compras, ventas, notas de crédito, notas de débito y ajustes.
    // Toda la lógica (validaciones, stock, kardex, costo de compra, IVA) está en sp_Transaccion_Registrar
    [Route("api/[controller]")]
    [ApiController]
    public class TransaccionesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TransaccionesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Transacciones?tipo=VENTA&idAlmacen=1&desde=2026-10-01&hasta=2026-10-31&estado=REGISTRADA
        // Devuelve solo los encabezados; el detalle se consulta con GET api/Transacciones/5
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TransaccionResponseDTO>>> GetTransacciones(
            string? tipo, int? idAlmacen, int? idCliente, int? idProveedor, DateTime? desde, DateTime? hasta, string? estado)
        {
            return await _context.Database.ConsultarAsync<TransaccionResponseDTO>("sp_Transaccion_Listar",
                Parametro("@Tipo", string.IsNullOrWhiteSpace(tipo) ? null : tipo),
                Parametro("@ID_Almacen", idAlmacen),
                Parametro("@ID_Cliente", idCliente),
                Parametro("@ID_Proveedor", idProveedor),
                Parametro("@Desde", desde?.Date),
                Parametro("@Hasta", hasta?.Date),
                Parametro("@Estado", string.IsNullOrWhiteSpace(estado) ? null : estado));
        }

        // GET: api/Transacciones/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TransaccionResponseDTO>> GetTransaccion(int id)
        {
            var transaccion = (await _context.Database.ConsultarAsync<TransaccionResponseDTO>("sp_Transaccion_Obtener",
                Parametro("@ID_Transaccion", id))).FirstOrDefault();

            if (transaccion == null)
            {
                return NotFound();
            }

            transaccion.Detalles = await _context.Database.ConsultarAsync<TransaccionDetalleResponseDTO>("sp_Transaccion_Detalle",
                Parametro("@ID_Transaccion", id));

            return transaccion;
        }

        // POST: api/Transacciones
        [HttpPost]
        public async Task<ActionResult<TransaccionResponseDTO>> PostTransaccion(TransaccionRequestDTO transaccionDto)
        {
            var id = await _context.Database.ObtenerIdAsync("sp_Transaccion_Registrar",
                Parametro("@CodigoTipoTransaccion", transaccionDto.CodigoTipoTransaccion),
                Parametro("@ID_Usuario", transaccionDto.ID_Usuario),
                Parametro("@ID_Almacen", transaccionDto.ID_Almacen),
                Parametro("@ID_Cliente", transaccionDto.ID_Cliente),
                Parametro("@ID_Proveedor", transaccionDto.ID_Proveedor),
                Parametro("@ID_OrdenCompra", transaccionDto.ID_OrdenCompra),
                Parametro("@ID_TransaccionOrigen", transaccionDto.ID_TransaccionOrigen),
                Parametro("@Referencia", transaccionDto.Referencia),
                Parametro("@Observaciones", transaccionDto.Observaciones),
                Parametro("@Detalles", Json(transaccionDto.Detalles)),
                Parametro("@DevuelveInventario", transaccionDto.DevuelveInventario));

            return CreatedAtAction("GetTransaccion", new { id }, (await GetTransaccion(id)).Value);
        }

        // POST: api/Transacciones/5/anular
        // Los documentos no se borran: se anulan y se generan los movimientos contrarios en el kardex
        [HttpPost("{id}/anular")]
        public async Task<IActionResult> AnularTransaccion(int id, AccionUsuarioRequestDTO anularDto)
        {
            await _context.Database.EjecutarAsync("sp_Transaccion_Anular",
                Parametro("@ID_Transaccion", id),
                Parametro("@ID_Usuario", anularDto.ID_Usuario),
                Parametro("@Motivo", anularDto.Motivo));

            return NoContent();
        }
    }
}
