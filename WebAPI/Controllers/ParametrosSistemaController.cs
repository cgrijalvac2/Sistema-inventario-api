using Microsoft.AspNetCore.Mvc;
using WebAPI.Contexts;
using WebAPI.Helpers;
using WebAPI.Models;
using static WebAPI.Helpers.SpHelper;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParametrosSistemaController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ParametrosSistemaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/ParametrosSistema
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ParametroSistema>>> GetParametrosSistema()
        {
            return await _context.ParametroSistema.ConsultarAsync("sp_ParametroSistema_Listar");
        }

        // GET: api/ParametrosSistema/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ParametroSistema>> GetParametroSistema(int id)
        {
            var parametro = (await _context.ParametroSistema.ConsultarAsync("sp_ParametroSistema_Obtener",
                Parametro("@ID_Parametro", id))).FirstOrDefault();

            if (parametro == null)
            {
                return NotFound();
            }

            return parametro;
        }

        // PUT: api/ParametrosSistema/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutParametroSistema(int id, ParametroSistema parametro)
        {
            if (id != parametro.ID_Parametro)
            {
                return BadRequest();
            }

            await _context.Database.EjecutarAsync("sp_ParametroSistema_Actualizar",
                Parametro("@ID_Parametro", id),
                Parametro("@ID_Almacen", parametro.ID_Almacen),
                Parametro("@Clave", parametro.Clave),
                Parametro("@Valor", parametro.Valor),
                Parametro("@Descripcion", parametro.Descripcion),
                Parametro("@Estado", parametro.Estado));

            return NoContent();
        }

        // POST: api/ParametrosSistema
        [HttpPost]
        public async Task<ActionResult<ParametroSistema>> PostParametroSistema(ParametroSistema parametro)
        {
            var nuevo = (await _context.ParametroSistema.ConsultarAsync("sp_ParametroSistema_Insertar",
                Parametro("@ID_Almacen", parametro.ID_Almacen),
                Parametro("@Clave", parametro.Clave),
                Parametro("@Valor", parametro.Valor),
                Parametro("@Descripcion", parametro.Descripcion))).First();

            return CreatedAtAction("GetParametroSistema", new { id = nuevo.ID_Parametro }, nuevo);
        }

        // DELETE: api/ParametrosSistema/5  (borrado lógico)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteParametroSistema(int id)
        {
            await _context.Database.EjecutarAsync("sp_ParametroSistema_Eliminar", Parametro("@ID_Parametro", id));

            return NoContent();
        }
    }
}
