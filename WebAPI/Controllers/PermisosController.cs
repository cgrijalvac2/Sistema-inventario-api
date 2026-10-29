using Microsoft.AspNetCore.Mvc;
using WebAPI.Contexts;
using WebAPI.Helpers;
using WebAPI.Models;
using static WebAPI.Helpers.SpHelper;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermisosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PermisosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Permisos?soloActivos=true
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Permiso>>> GetPermisos(bool soloActivos = false)
        {
            return await _context.Permiso.ConsultarAsync("sp_Permiso_Listar",
                Parametro("@SoloActivos", soloActivos));
        }

        // GET: api/Permisos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Permiso>> GetPermiso(int id)
        {
            var permiso = (await _context.Permiso.ConsultarAsync("sp_Permiso_Obtener",
                Parametro("@ID_Permiso", id))).FirstOrDefault();

            if (permiso == null)
            {
                return NotFound();
            }

            return permiso;
        }

        // PUT: api/Permisos/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPermiso(int id, Permiso permiso)
        {
            if (id != permiso.ID_Permiso)
            {
                return BadRequest();
            }

            await _context.Database.EjecutarAsync("sp_Permiso_Actualizar",
                Parametro("@ID_Permiso", id),
                Parametro("@Codigo", permiso.Codigo),
                Parametro("@Nombre", permiso.Nombre),
                Parametro("@Descripcion", permiso.Descripcion),
                Parametro("@Estado", permiso.Estado));

            return NoContent();
        }

        // POST: api/Permisos
        [HttpPost]
        public async Task<ActionResult<Permiso>> PostPermiso(Permiso permiso)
        {
            var nuevo = (await _context.Permiso.ConsultarAsync("sp_Permiso_Insertar",
                Parametro("@Codigo", permiso.Codigo),
                Parametro("@Nombre", permiso.Nombre),
                Parametro("@Descripcion", permiso.Descripcion))).First();

            return CreatedAtAction("GetPermiso", new { id = nuevo.ID_Permiso }, nuevo);
        }

        // DELETE: api/Permisos/5  (borrado lógico)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePermiso(int id)
        {
            await _context.Database.EjecutarAsync("sp_Permiso_Eliminar", Parametro("@ID_Permiso", id));

            return NoContent();
        }
    }
}
