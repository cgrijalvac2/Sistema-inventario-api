using Microsoft.AspNetCore.Mvc;
using WebAPI.Contexts;
using WebAPI.DTOs;
using WebAPI.Helpers;
using static WebAPI.Helpers.SpHelper;

namespace WebAPI.Controllers
{
    // La orden de compra es solo el pedido. La mercadería entra al inventario cuando se registra
    // una COMPRA en api/Transacciones indicando el ID_OrdenCompra
    [Route("api/[controller]")]
    [ApiController]
    public class OrdenesCompraController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OrdenesCompraController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/OrdenesCompra?estado=PENDIENTE&idProveedor=1&idAlmacen=1
        // Devuelve solo los encabezados; el detalle se consulta con GET api/OrdenesCompra/5
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrdenCompraResponseDTO>>> GetOrdenesCompra(string? estado, int? idProveedor, int? idAlmacen)
        {
            return await _context.Database.ConsultarAsync<OrdenCompraResponseDTO>("sp_OrdenCompra_Listar",
                Parametro("@Estado", string.IsNullOrWhiteSpace(estado) ? null : estado),
                Parametro("@ID_Proveedor", idProveedor),
                Parametro("@ID_Almacen", idAlmacen));
        }

        // GET: api/OrdenesCompra/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OrdenCompraResponseDTO>> GetOrdenCompra(int id)
        {
            var orden = (await _context.Database.ConsultarAsync<OrdenCompraResponseDTO>("sp_OrdenCompra_Obtener",
                Parametro("@ID_OrdenCompra", id))).FirstOrDefault();

            if (orden == null)
            {
                return NotFound();
            }

            orden.Detalles = await _context.Database.ConsultarAsync<OrdenCompraDetalleResponseDTO>("sp_OrdenCompra_Detalle",
                Parametro("@ID_OrdenCompra", id));

            return orden;
        }

        // POST: api/OrdenesCompra
        [HttpPost]
        public async Task<ActionResult<OrdenCompraResponseDTO>> PostOrdenCompra(OrdenCompraRequestDTO ordenDto)
        {
            var id = await Guardar(null, ordenDto);

            return CreatedAtAction("GetOrdenCompra", new { id }, (await GetOrdenCompra(id)).Value);
        }

        // PUT: api/OrdenesCompra/5  (solo mientras esté PENDIENTE)
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrdenCompra(int id, OrdenCompraRequestDTO ordenDto)
        {
            await Guardar(id, ordenDto);

            return NoContent();
        }

        // POST: api/OrdenesCompra/5/cancelar
        [HttpPost("{id}/cancelar")]
        public async Task<IActionResult> CancelarOrdenCompra(int id)
        {
            await _context.Database.EjecutarAsync("sp_OrdenCompra_Cancelar", Parametro("@ID_OrdenCompra", id));

            return NoContent();
        }

        // El mismo SP crea (id = null) o modifica la orden. El detalle viaja como JSON
        private Task<int> Guardar(int? id, OrdenCompraRequestDTO ordenDto)
        {
            return _context.Database.ObtenerIdAsync("sp_OrdenCompra_Guardar",
                Parametro("@ID_OrdenCompra", id),
                Parametro("@ID_Proveedor", ordenDto.ID_Proveedor),
                Parametro("@ID_Almacen", ordenDto.ID_Almacen),
                Parametro("@ID_Usuario", ordenDto.ID_Usuario),
                Parametro("@FechaEsperada", ordenDto.FechaEsperada?.Date),
                Parametro("@Observaciones", ordenDto.Observaciones),
                Parametro("@Detalles", Json(ordenDto.Detalles)));
        }
    }
}
