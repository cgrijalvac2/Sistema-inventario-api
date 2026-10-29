using Microsoft.AspNetCore.Mvc;
using WebAPI.Contexts;
using WebAPI.DTOs;
using WebAPI.Helpers;
using static WebAPI.Helpers.SpHelper;

namespace WebAPI.Controllers
{
    // Consultas y reportes. El stock no se modifica aquí: solo lo cambian transacciones y transferencias
    [Route("api/[controller]")]
    [ApiController]
    public class InventarioController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public InventarioController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Inventario/existencias?idAlmacen=1&idCategoria=2&buscar=agua&soloConStock=true
        // Listado de existencias
        [HttpGet("existencias")]
        public async Task<ActionResult<IEnumerable<ExistenciaResponseDTO>>> GetExistencias(
            int? idAlmacen, int? idCategoria, string? buscar, bool soloConStock = false)
        {
            return await _context.Database.ConsultarAsync<ExistenciaResponseDTO>("sp_Inventario_Existencias",
                Parametro("@ID_Almacen", idAlmacen),
                Parametro("@ID_Categoria", idCategoria),
                Parametro("@Buscar", string.IsNullOrWhiteSpace(buscar) ? null : buscar),
                Parametro("@SoloConStock", soloConStock),
                Parametro("@SoloBajo", false));
        }

        // GET: api/Inventario/stock-bajo?idAlmacen=1
        // Productos con existencia igual o menor al mínimo (alertas de reabastecimiento)
        [HttpGet("stock-bajo")]
        public async Task<ActionResult<IEnumerable<ExistenciaResponseDTO>>> GetStockBajo(int? idAlmacen)
        {
            return await _context.Database.ConsultarAsync<ExistenciaResponseDTO>("sp_Inventario_Existencias",
                Parametro("@ID_Almacen", idAlmacen),
                Parametro("@SoloBajo", true));
        }

        // GET: api/Inventario/kardex?idProducto=1&idAlmacen=1&desde=2026-10-01&hasta=2026-10-31
        // Historial de un producto en orden cronológico
        [HttpGet("kardex")]
        public async Task<ActionResult<IEnumerable<MovimientoResponseDTO>>> GetKardex(int idProducto, int? idAlmacen, DateTime? desde, DateTime? hasta)
        {
            return await _context.Database.ConsultarAsync<MovimientoResponseDTO>("sp_Inventario_Movimientos",
                Parametro("@ID_Producto", idProducto),
                Parametro("@ID_Almacen", idAlmacen),
                Parametro("@Desde", desde?.Date),
                Parametro("@Hasta", hasta?.Date));
        }

        // GET: api/Inventario/movimientos?tipoMovimiento=ENTRADA&idAlmacen=1&desde=2026-10-01&hasta=2026-10-31
        // Registro de movimientos de entrada y salida de todos los productos (más recientes primero)
        [HttpGet("movimientos")]
        public async Task<ActionResult<IEnumerable<MovimientoResponseDTO>>> GetMovimientos(
            string? tipoMovimiento, int? idAlmacen, DateTime? desde, DateTime? hasta)
        {
            return await _context.Database.ConsultarAsync<MovimientoResponseDTO>("sp_Inventario_Movimientos",
                Parametro("@ID_Almacen", idAlmacen),
                Parametro("@TipoMovimiento", string.IsNullOrWhiteSpace(tipoMovimiento) ? null : tipoMovimiento),
                Parametro("@Desde", desde?.Date),
                Parametro("@Hasta", hasta?.Date));
        }

        // GET: api/Inventario/resumen
        // Total de productos, unidades y valor del inventario por almacén
        [HttpGet("resumen")]
        public async Task<ActionResult<IEnumerable<ResumenAlmacenDTO>>> GetResumen()
        {
            return await _context.Database.ConsultarAsync<ResumenAlmacenDTO>("sp_Inventario_Resumen");
        }

        // PUT: api/Inventario/limites/1/5   (idAlmacen / idProducto)
        // Define el stock mínimo y máximo de un producto en un almacén
        [HttpPut("limites/{idAlmacen}/{idProducto}")]
        public async Task<IActionResult> PutLimites(int idAlmacen, int idProducto, LimitesStockRequestDTO limitesDto)
        {
            await _context.Database.EjecutarAsync("sp_Inventario_ActualizarLimites",
                Parametro("@ID_Almacen", idAlmacen),
                Parametro("@ID_Producto", idProducto),
                Parametro("@StockMinimo", limitesDto.StockMinimo),
                Parametro("@StockMaximo", limitesDto.StockMaximo));

            return NoContent();
        }
    }
}
