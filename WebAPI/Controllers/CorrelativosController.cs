using Microsoft.AspNetCore.Mvc;
using WebAPI.Contexts;
using WebAPI.Helpers;
using WebAPI.Models;
using static WebAPI.Helpers.SpHelper;

namespace WebAPI.Controllers
{
    // Solo consulta y modificación: las claves las usa el sistema internamente, por eso no se crean ni borran
    [Route("api/[controller]")]
    [ApiController]
    public class CorrelativosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CorrelativosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Correlativos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Correlativo>>> GetCorrelativos()
        {
            return await _context.Correlativo.ConsultarAsync("sp_Correlativo_Listar");
        }

        // GET: api/Correlativos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Correlativo>> GetCorrelativo(int id)
        {
            var correlativo = (await _context.Correlativo.ConsultarAsync("sp_Correlativo_Obtener",
                Parametro("@ID_Correlativo", id))).FirstOrDefault();

            if (correlativo == null)
            {
                return NotFound();
            }

            return correlativo;
        }

        // PUT: api/Correlativos/5  (prefijo y número desde el que se continúa)
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCorrelativo(int id, Correlativo correlativo)
        {
            await _context.Database.EjecutarAsync("sp_Correlativo_Actualizar",
                Parametro("@ID_Correlativo", id),
                Parametro("@Prefijo", correlativo.Prefijo),
                Parametro("@UltimoNumero", correlativo.UltimoNumero),
                Parametro("@Estado", correlativo.Estado));

            return NoContent();
        }
    }
}
