using Microsoft.AspNetCore.Mvc;
using WebAPI.Contexts;
using WebAPI.Helpers;
using WebAPI.Models;
using static WebAPI.Helpers.SpHelper;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MotivosAjusteController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MotivosAjusteController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/MotivosAjuste?soloActivos=true
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MotivoAjuste>>> GetMotivosAjuste(bool soloActivos = false)
        {
            return await _context.MotivoAjuste.ConsultarAsync("sp_MotivoAjuste_Listar",
                Parametro("@SoloActivos", soloActivos));
        }

        // GET: api/MotivosAjuste/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MotivoAjuste>> GetMotivoAjuste(int id)
        {
            var motivoAjuste = (await _context.MotivoAjuste.ConsultarAsync("sp_MotivoAjuste_Obtener",
                Parametro("@ID_Motivo", id))).FirstOrDefault();

            if (motivoAjuste == null)
            {
                return NotFound();
            }

            return motivoAjuste;
        }

        // PUT: api/MotivosAjuste/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMotivoAjuste(int id, MotivoAjuste motivoAjuste)
        {
            if (id != motivoAjuste.ID_Motivo)
            {
                return BadRequest();
            }

            await _context.Database.EjecutarAsync("sp_MotivoAjuste_Actualizar",
                Parametro("@ID_Motivo", id),
                Parametro("@Descripcion", motivoAjuste.Descripcion),
                Parametro("@Estado", motivoAjuste.Estado));

            return NoContent();
        }

        // POST: api/MotivosAjuste
        [HttpPost]
        public async Task<ActionResult<MotivoAjuste>> PostMotivoAjuste(MotivoAjuste motivoAjuste)
        {
            var nuevo = (await _context.MotivoAjuste.ConsultarAsync("sp_MotivoAjuste_Insertar",
                Parametro("@Descripcion", motivoAjuste.Descripcion))).First();

            return CreatedAtAction("GetMotivoAjuste", new { id = nuevo.ID_Motivo }, nuevo);
        }

        // DELETE: api/MotivosAjuste/5  (borrado lógico)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMotivoAjuste(int id)
        {
            await _context.Database.EjecutarAsync("sp_MotivoAjuste_Eliminar", Parametro("@ID_Motivo", id));

            return NoContent();
        }
    }
}
