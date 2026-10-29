using Microsoft.AspNetCore.Mvc;
using WebAPI.Contexts;
using WebAPI.Helpers;
using WebAPI.Models;
using static WebAPI.Helpers.SpHelper;

namespace WebAPI.Controllers
{
    // Los tipos se crean con los datos iniciales. Solo se permite cambiar nombre, descripción y estado,
    // porque el Factor_Kardex y las reglas definen cómo se comporta el inventario
    [Route("api/[controller]")]
    [ApiController]
    public class TiposTransaccionController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TiposTransaccionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/TiposTransaccion?soloActivos=true
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TipoTransaccion>>> GetTiposTransaccion(bool soloActivos = false)
        {
            return await _context.TipoTransaccion.ConsultarAsync("sp_TipoTransaccion_Listar",
                Parametro("@SoloActivos", soloActivos));
        }

        // GET: api/TiposTransaccion/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TipoTransaccion>> GetTipoTransaccion(int id)
        {
            var tipo = (await _context.TipoTransaccion.ConsultarAsync("sp_TipoTransaccion_Obtener",
                Parametro("@ID_TipoTransaccion", id))).FirstOrDefault();

            if (tipo == null)
            {
                return NotFound();
            }

            return tipo;
        }

        // PUT: api/TiposTransaccion/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTipoTransaccion(int id, TipoTransaccion tipo)
        {
            await _context.Database.EjecutarAsync("sp_TipoTransaccion_Actualizar",
                Parametro("@ID_TipoTransaccion", id),
                Parametro("@Nombre", tipo.Nombre),
                Parametro("@Descripcion", tipo.Descripcion),
                Parametro("@Estado", tipo.Estado));

            return NoContent();
        }
    }
}
