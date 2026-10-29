using Microsoft.AspNetCore.Mvc;
using WebAPI.Contexts;
using WebAPI.DTOs;
using WebAPI.Helpers;
using static WebAPI.Helpers.SpHelper;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Productos?buscar=agua&idCategoria=1&soloActivos=true
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductoResponseDTO>>> GetProductos(string? buscar, int? idCategoria, bool soloActivos = false)
        {
            return await _context.Database.ConsultarAsync<ProductoResponseDTO>("sp_Producto_Listar",
                Parametro("@Buscar", string.IsNullOrWhiteSpace(buscar) ? null : buscar),
                Parametro("@ID_Categoria", idCategoria),
                Parametro("@SoloActivos", soloActivos));
        }

        // GET: api/Productos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductoResponseDTO>> GetProducto(int id)
        {
            var producto = (await _context.Database.ConsultarAsync<ProductoResponseDTO>("sp_Producto_Obtener",
                Parametro("@ID_Producto", id))).FirstOrDefault();

            if (producto == null)
            {
                return NotFound();
            }

            return producto;
        }

        // POST: api/Productos
        [HttpPost]
        public async Task<ActionResult<ProductoResponseDTO>> PostProducto(ProductoRequestDTO productoDto)
        {
            var id = await _context.Database.ObtenerIdAsync("sp_Producto_Insertar",
                Parametro("@CodigoProducto", productoDto.CodigoProducto),
                Parametro("@NombreProducto", productoDto.NombreProducto),
                Parametro("@Descripcion", productoDto.Descripcion),
                Parametro("@ID_Categoria", productoDto.ID_Categoria),
                Parametro("@ID_UnidadMedida", productoDto.ID_UnidadMedida),
                Parametro("@Costo", productoDto.Costo),
                Parametro("@Precio", productoDto.Precio));

            return CreatedAtAction("GetProducto", new { id }, (await GetProducto(id)).Value);
        }

        // PUT: api/Productos/5  (el costo no se modifica aquí: lo calculan las compras)
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProducto(int id, ProductoRequestDTO productoDto)
        {
            await _context.Database.EjecutarAsync("sp_Producto_Actualizar",
                Parametro("@ID_Producto", id),
                Parametro("@CodigoProducto", productoDto.CodigoProducto),
                Parametro("@NombreProducto", productoDto.NombreProducto),
                Parametro("@Descripcion", productoDto.Descripcion),
                Parametro("@ID_Categoria", productoDto.ID_Categoria),
                Parametro("@ID_UnidadMedida", productoDto.ID_UnidadMedida),
                Parametro("@Precio", productoDto.Precio));

            return NoContent();
        }

        // DELETE: api/Productos/5  (borrado lógico)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProducto(int id)
        {
            await _context.Database.EjecutarAsync("sp_Producto_CambiarEstado",
                Parametro("@ID_Producto", id), Parametro("@Estado", false));

            return NoContent();
        }

        // PATCH: api/Productos/5/activar
        [HttpPatch("{id}/activar")]
        public async Task<IActionResult> ActivarProducto(int id)
        {
            await _context.Database.EjecutarAsync("sp_Producto_CambiarEstado",
                Parametro("@ID_Producto", id), Parametro("@Estado", true));

            return NoContent();
        }
    }
}
