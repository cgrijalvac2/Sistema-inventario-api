using Microsoft.AspNetCore.Mvc;
using WebAPI.Contexts;
using WebAPI.DTOs;
using WebAPI.Helpers;
using static WebAPI.Helpers.SpHelper;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProveedoresController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProveedoresController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Proveedores?buscar=distribuidora&soloActivos=true
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProveedorResponseDTO>>> GetProveedores(string? buscar, bool soloActivos = false)
        {
            return await _context.Database.ConsultarAsync<ProveedorResponseDTO>("sp_Proveedor_Listar",
                Parametro("@Buscar", string.IsNullOrWhiteSpace(buscar) ? null : buscar),
                Parametro("@SoloActivos", soloActivos));
        }

        // GET: api/Proveedores/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProveedorResponseDTO>> GetProveedor(int id)
        {
            var proveedor = (await _context.Database.ConsultarAsync<ProveedorResponseDTO>("sp_Proveedor_Obtener",
                Parametro("@ID_Proveedor", id))).FirstOrDefault();

            if (proveedor == null)
            {
                return NotFound();
            }

            return proveedor;
        }

        // POST: api/Proveedores
        [HttpPost]
        public async Task<ActionResult<ProveedorResponseDTO>> PostProveedor(ProveedorRequestDTO proveedorDto)
        {
            var id = await _context.Database.ObtenerIdAsync("sp_Proveedor_Insertar",
                Parametro("@Nombre", proveedorDto.Nombre),
                Parametro("@NIT", proveedorDto.NIT),
                Parametro("@Direccion", proveedorDto.Direccion),
                Parametro("@Telefono", proveedorDto.Telefono),
                Parametro("@CorreoElectronico", proveedorDto.CorreoElectronico),
                Parametro("@DiasCredito", proveedorDto.DiasCredito));

            return CreatedAtAction("GetProveedor", new { id }, (await GetProveedor(id)).Value);
        }

        // PUT: api/Proveedores/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProveedor(int id, ProveedorRequestDTO proveedorDto)
        {
            await _context.Database.EjecutarAsync("sp_Proveedor_Actualizar",
                Parametro("@ID_Proveedor", id),
                Parametro("@Nombre", proveedorDto.Nombre),
                Parametro("@NIT", proveedorDto.NIT),
                Parametro("@Direccion", proveedorDto.Direccion),
                Parametro("@Telefono", proveedorDto.Telefono),
                Parametro("@CorreoElectronico", proveedorDto.CorreoElectronico),
                Parametro("@DiasCredito", proveedorDto.DiasCredito));

            return NoContent();
        }

        // DELETE: api/Proveedores/5  (borrado lógico)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProveedor(int id)
        {
            await _context.Database.EjecutarAsync("sp_Proveedor_CambiarEstado",
                Parametro("@ID_Proveedor", id), Parametro("@Estado", false));

            return NoContent();
        }

        // PATCH: api/Proveedores/5/activar
        [HttpPatch("{id}/activar")]
        public async Task<IActionResult> ActivarProveedor(int id)
        {
            await _context.Database.EjecutarAsync("sp_Proveedor_CambiarEstado",
                Parametro("@ID_Proveedor", id), Parametro("@Estado", true));

            return NoContent();
        }
    }
}
