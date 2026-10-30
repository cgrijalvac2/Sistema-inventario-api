using Microsoft.AspNetCore.Mvc;
using WebAPI.Contexts;
using WebAPI.DTOs;
using WebAPI.Helpers;
using static WebAPI.Helpers.SpHelper;

namespace WebAPI.Controllers
{
    // Mantenimiento de la tabla InventarioAlmacen: qué productos maneja cada almacén y sus límites.
    // No modifica la existencia (eso solo lo hacen los documentos)
    [Route("api/[controller]")]
    [ApiController]
    public class InventarioAlmacenController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public InventarioAlmacenController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/InventarioAlmacen?idAlmacen=1&buscar=agua
        // Productos asignados al almacén (incluye los que tienen existencia 0)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExistenciaResponseDTO>>> GetProductosAlmacen(int idAlmacen, string? buscar)
        {
            return await _context.Database.ConsultarAsync<ExistenciaResponseDTO>("sp_Inventario_Existencias",
                Parametro("@ID_Almacen", idAlmacen),
                Parametro("@Buscar", string.IsNullOrWhiteSpace(buscar) ? null : buscar));
        }

        // POST: api/InventarioAlmacen
        // Asigna uno o varios productos al almacén con su existencia actual y límites.
        // La existencia se registra como un ajuste de entrada (Inventario inicial). Devuelve cuántos se agregaron
        [HttpPost]
        public async Task<ActionResult> PostAsignar(AsignarProductosRequestDTO asignarDto)
        {
            var agregados = await _context.Database.ObtenerIdAsync("sp_InventarioAlmacen_Asignar",
                Parametro("@ID_Almacen", asignarDto.ID_Almacen),
                Parametro("@ID_Usuario", asignarDto.ID_Usuario),
                Parametro("@Productos", Json(asignarDto.Productos)));

            return Ok(new { agregados });
        }

        // PUT: api/InventarioAlmacen/1/5   (idAlmacen / idProducto)
        // Cambia los límites y, si se manda CantidadActual, ajusta la existencia (conteo físico)
        [HttpPut("{idAlmacen}/{idProducto}")]
        public async Task<IActionResult> PutProductoAlmacen(int idAlmacen, int idProducto, ActualizarProductoAlmacenRequestDTO actualizarDto)
        {
            await _context.Database.EjecutarAsync("sp_InventarioAlmacen_Actualizar",
                Parametro("@ID_Almacen", idAlmacen),
                Parametro("@ID_Producto", idProducto),
                Parametro("@ID_Usuario", actualizarDto.ID_Usuario),
                Parametro("@StockMinimo", actualizarDto.StockMinimo),
                Parametro("@StockMaximo", actualizarDto.StockMaximo),
                Parametro("@CantidadActual", actualizarDto.CantidadActual));

            return NoContent();
        }

        // DELETE: api/InventarioAlmacen/1/5
        // Quita el producto del almacén (solo si su existencia es 0)
        [HttpDelete("{idAlmacen}/{idProducto}")]
        public async Task<IActionResult> DeleteProductoAlmacen(int idAlmacen, int idProducto)
        {
            await _context.Database.EjecutarAsync("sp_InventarioAlmacen_Quitar",
                Parametro("@ID_Almacen", idAlmacen),
                Parametro("@ID_Producto", idProducto));

            return NoContent();
        }
    }
}
