using Microsoft.AspNetCore.Mvc;
using WebAPI.Contexts;
using WebAPI.DTOs;
using WebAPI.Helpers;
using static WebAPI.Helpers.SpHelper;

namespace WebAPI.Controllers
{
    // Datos para la pantalla de inicio del frontend
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Dashboard/indicadores?idAlmacen=1  (sin idAlmacen = todos los almacenes)
        [HttpGet("indicadores")]
        public async Task<ActionResult<IndicadoresDTO>> GetIndicadores(int? idAlmacen)
        {
            var indicadores = await _context.Database.ConsultarAsync<IndicadoresDTO>("sp_Dashboard_Indicadores",
                Parametro("@ID_Almacen", idAlmacen));

            return indicadores.First();
        }

        // GET: api/Dashboard/ventas-por-dia?idAlmacen=1&dias=7
        [HttpGet("ventas-por-dia")]
        public async Task<ActionResult<IEnumerable<VentaDiaDTO>>> GetVentasPorDia(int? idAlmacen, int dias = 7)
        {
            return await _context.Database.ConsultarAsync<VentaDiaDTO>("sp_Dashboard_VentasPorDia",
                Parametro("@ID_Almacen", idAlmacen),
                Parametro("@Dias", Math.Clamp(dias, 1, 90)));
        }

        // GET: api/Dashboard/top-productos?idAlmacen=1
        [HttpGet("top-productos")]
        public async Task<ActionResult<IEnumerable<TopProductoDTO>>> GetTopProductos(int? idAlmacen)
        {
            return await _context.Database.ConsultarAsync<TopProductoDTO>("sp_Dashboard_TopProductos",
                Parametro("@ID_Almacen", idAlmacen));
        }
    }
}
