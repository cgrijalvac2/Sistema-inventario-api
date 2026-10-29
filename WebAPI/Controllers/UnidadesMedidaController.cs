using Microsoft.AspNetCore.Mvc;
using WebAPI.Contexts;
using WebAPI.Helpers;
using WebAPI.Models;
using static WebAPI.Helpers.SpHelper;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UnidadesMedidaController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UnidadesMedidaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/UnidadesMedida?soloActivos=true
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UnidadMedida>>> GetUnidadesMedida(bool soloActivos = false)
        {
            return await _context.UnidadMedida.ConsultarAsync("sp_UnidadMedida_Listar",
                Parametro("@SoloActivos", soloActivos));
        }

        // GET: api/UnidadesMedida/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UnidadMedida>> GetUnidadMedida(int id)
        {
            var unidadMedida = (await _context.UnidadMedida.ConsultarAsync("sp_UnidadMedida_Obtener",
                Parametro("@ID_UnidadMedida", id))).FirstOrDefault();

            if (unidadMedida == null)
            {
                return NotFound();
            }

            return unidadMedida;
        }

        // PUT: api/UnidadesMedida/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUnidadMedida(int id, UnidadMedida unidadMedida)
        {
            if (id != unidadMedida.ID_UnidadMedida)
            {
                return BadRequest();
            }

            await _context.Database.EjecutarAsync("sp_UnidadMedida_Actualizar",
                Parametro("@ID_UnidadMedida", id),
                Parametro("@Nombre", unidadMedida.Nombre),
                Parametro("@Abreviatura", unidadMedida.Abreviatura),
                Parametro("@Estado", unidadMedida.Estado));

            return NoContent();
        }

        // POST: api/UnidadesMedida
        [HttpPost]
        public async Task<ActionResult<UnidadMedida>> PostUnidadMedida(UnidadMedida unidadMedida)
        {
            var nueva = (await _context.UnidadMedida.ConsultarAsync("sp_UnidadMedida_Insertar",
                Parametro("@Nombre", unidadMedida.Nombre),
                Parametro("@Abreviatura", unidadMedida.Abreviatura))).First();

            return CreatedAtAction("GetUnidadMedida", new { id = nueva.ID_UnidadMedida }, nueva);
        }

        // DELETE: api/UnidadesMedida/5  (borrado lógico)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUnidadMedida(int id)
        {
            await _context.Database.EjecutarAsync("sp_UnidadMedida_Eliminar", Parametro("@ID_UnidadMedida", id));

            return NoContent();
        }
    }
}
