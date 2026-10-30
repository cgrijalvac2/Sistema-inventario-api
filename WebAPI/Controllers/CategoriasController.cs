using Microsoft.AspNetCore.Mvc;
using WebAPI.Contexts;
using WebAPI.Helpers;
using WebAPI.Models;
using static WebAPI.Helpers.SpHelper;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CategoriasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Categorias?soloActivos=true
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Categoria>>> GetCategorias(bool soloActivos = false)
        {
            return await _context.Categoria.ConsultarAsync("sp_Categoria_Listar",
                Parametro("@SoloActivos", soloActivos));
        }

        // GET: api/Categorias/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Categoria>> GetCategoria(int id)
        {
            var categoria = (await _context.Categoria.ConsultarAsync("sp_Categoria_Obtener",
                Parametro("@ID_Categoria", id))).FirstOrDefault();

            if (categoria == null)
            {
                return NotFound();
            }

            return categoria;
        }

        // PUT: api/Categorias/5  (con Estado = true también sirve para reactivar)
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategoria(int id, Categoria categoria)
        {
            if (id != categoria.ID_Categoria)
            {
                return BadRequest();
            }

            await _context.Database.EjecutarAsync("sp_Categoria_Actualizar",
                Parametro("@ID_Categoria", id),
                Parametro("@NombreCategoria", categoria.NombreCategoria),
                Parametro("@DescripcionCategoria", categoria.DescripcionCategoria),
                Parametro("@Estado", categoria.Estado));

            return NoContent();
        }

        // POST: api/Categorias
        [HttpPost]
        public async Task<ActionResult<Categoria>> PostCategoria(Categoria categoria)
        {
            var nueva = (await _context.Categoria.ConsultarAsync("sp_Categoria_Insertar",
                Parametro("@NombreCategoria", categoria.NombreCategoria),
                Parametro("@DescripcionCategoria", categoria.DescripcionCategoria),
                Parametro("@Estado", categoria.Estado))).First();

            return CreatedAtAction("GetCategoria", new { id = nueva.ID_Categoria }, nueva);
        }

        // DELETE: api/Categorias/5  (borrado lógico)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategoria(int id)
        {
            await _context.Database.EjecutarAsync("sp_Categoria_Eliminar", Parametro("@ID_Categoria", id));

            return NoContent();
        }
    }
}
