using Microsoft.AspNetCore.Mvc;
using WebAPI.Contexts;
using WebAPI.DTOs;
using WebAPI.Helpers;
using static WebAPI.Helpers.SpHelper;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ClientesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Clientes?buscar=juan&soloActivos=true
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClienteResponseDTO>>> GetClientes(string? buscar, bool soloActivos = false)
        {
            return await _context.Database.ConsultarAsync<ClienteResponseDTO>("sp_Cliente_Listar",
                Parametro("@Buscar", string.IsNullOrWhiteSpace(buscar) ? null : buscar),
                Parametro("@SoloActivos", soloActivos));
        }

        // GET: api/Clientes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ClienteResponseDTO>> GetCliente(int id)
        {
            var cliente = (await _context.Database.ConsultarAsync<ClienteResponseDTO>("sp_Cliente_Obtener",
                Parametro("@ID_Cliente", id))).FirstOrDefault();

            if (cliente == null)
            {
                return NotFound();
            }

            return cliente;
        }

        // POST: api/Clientes
        [HttpPost]
        public async Task<ActionResult<ClienteResponseDTO>> PostCliente(ClienteRequestDTO clienteDto)
        {
            var id = await _context.Database.ObtenerIdAsync("sp_Cliente_Insertar",
                Parametro("@Nombre", clienteDto.Nombre),
                Parametro("@NIT", clienteDto.NIT),
                Parametro("@Direccion", clienteDto.Direccion),
                Parametro("@Telefono", clienteDto.Telefono),
                Parametro("@CorreoElectronico", clienteDto.CorreoElectronico));

            return CreatedAtAction("GetCliente", new { id }, (await GetCliente(id)).Value);
        }

        // PUT: api/Clientes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCliente(int id, ClienteRequestDTO clienteDto)
        {
            await _context.Database.EjecutarAsync("sp_Cliente_Actualizar",
                Parametro("@ID_Cliente", id),
                Parametro("@Nombre", clienteDto.Nombre),
                Parametro("@NIT", clienteDto.NIT),
                Parametro("@Direccion", clienteDto.Direccion),
                Parametro("@Telefono", clienteDto.Telefono),
                Parametro("@CorreoElectronico", clienteDto.CorreoElectronico));

            return NoContent();
        }

        // DELETE: api/Clientes/5  (borrado lógico)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCliente(int id)
        {
            await _context.Database.EjecutarAsync("sp_Cliente_CambiarEstado",
                Parametro("@ID_Cliente", id), Parametro("@Estado", false));

            return NoContent();
        }

        // PATCH: api/Clientes/5/activar
        [HttpPatch("{id}/activar")]
        public async Task<IActionResult> ActivarCliente(int id)
        {
            await _context.Database.EjecutarAsync("sp_Cliente_CambiarEstado",
                Parametro("@ID_Cliente", id), Parametro("@Estado", true));

            return NoContent();
        }
    }
}
