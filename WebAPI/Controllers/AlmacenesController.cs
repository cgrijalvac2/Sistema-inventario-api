using Microsoft.AspNetCore.Mvc;
using WebAPI.Contexts;
using WebAPI.Helpers;
using WebAPI.Models;
using static WebAPI.Helpers.SpHelper;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlmacenesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AlmacenesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Almacenes?soloActivos=true
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Almacen>>> GetAlmacenes(bool soloActivos = false)
        {
            return await _context.Almacen.ConsultarAsync("sp_Almacen_Listar",
                Parametro("@SoloActivos", soloActivos));
        }

        // GET: api/Almacenes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Almacen>> GetAlmacen(int id)
        {
            var almacen = (await _context.Almacen.ConsultarAsync("sp_Almacen_Obtener",
                Parametro("@ID_Almacen", id))).FirstOrDefault();

            if (almacen == null)
            {
                return NotFound();
            }

            return almacen;
        }

        // PUT: api/Almacenes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAlmacen(int id, Almacen almacen)
        {
            if (id != almacen.ID_Almacen)
            {
                return BadRequest();
            }

            await _context.Database.EjecutarAsync("sp_Almacen_Actualizar",
                Parametro("@ID_Almacen", id),
                Parametro("@Codigo", almacen.Codigo),
                Parametro("@Nombre", almacen.Nombre),
                Parametro("@Direccion", almacen.Direccion),
                Parametro("@Descripcion", almacen.Descripcion),
                Parametro("@Estado", almacen.Estado));

            return NoContent();
        }

        // POST: api/Almacenes
        [HttpPost]
        public async Task<ActionResult<Almacen>> PostAlmacen(Almacen almacen)
        {
            var nuevo = (await _context.Almacen.ConsultarAsync("sp_Almacen_Insertar",
                Parametro("@Codigo", almacen.Codigo),
                Parametro("@Nombre", almacen.Nombre),
                Parametro("@Direccion", almacen.Direccion),
                Parametro("@Descripcion", almacen.Descripcion),
                Parametro("@Estado", almacen.Estado))).First();

            return CreatedAtAction("GetAlmacen", new { id = nuevo.ID_Almacen }, nuevo);
        }

        // DELETE: api/Almacenes/5  (borrado lógico; no se permite si tiene existencias)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAlmacen(int id)
        {
            await _context.Database.EjecutarAsync("sp_Almacen_Eliminar", Parametro("@ID_Almacen", id));

            return NoContent();
        }
    }
}
