using Microsoft.AspNetCore.Mvc;
using WebAPI.Contexts;
using WebAPI.DTOs;
using WebAPI.Helpers;
using static WebAPI.Helpers.SpHelper;

namespace WebAPI.Controllers
{
    // Flujo: se crea (ENVIADA, sale del almacén origen) -> el destino la RECIBE (entra al destino)
    //        o la RECHAZA (la mercadería regresa al origen)
    [Route("api/[controller]")]
    [ApiController]
    public class TransferenciasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TransferenciasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Transferencias?idAlmacen=1&estado=ENVIADA
        // idAlmacen filtra las que salen de ese almacén o llegan a él
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TransferenciaResponseDTO>>> GetTransferencias(int? idAlmacen, string? estado)
        {
            return await _context.Database.ConsultarAsync<TransferenciaResponseDTO>("sp_Transferencia_Listar",
                Parametro("@ID_Almacen", idAlmacen),
                Parametro("@Estado", string.IsNullOrWhiteSpace(estado) ? null : estado));
        }

        // GET: api/Transferencias/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TransferenciaResponseDTO>> GetTransferencia(int id)
        {
            var transferencia = (await _context.Database.ConsultarAsync<TransferenciaResponseDTO>("sp_Transferencia_Obtener",
                Parametro("@ID_Transferencia", id))).FirstOrDefault();

            if (transferencia == null)
            {
                return NotFound();
            }

            transferencia.Detalles = await _context.Database.ConsultarAsync<TransferenciaDetalleResponseDTO>("sp_Transferencia_Detalle",
                Parametro("@ID_Transferencia", id));

            return transferencia;
        }

        // POST: api/Transferencias
        [HttpPost]
        public async Task<ActionResult<TransferenciaResponseDTO>> PostTransferencia(TransferenciaRequestDTO transferenciaDto)
        {
            var id = await _context.Database.ObtenerIdAsync("sp_Transferencia_Registrar",
                Parametro("@ID_AlmacenOrigen", transferenciaDto.ID_AlmacenOrigen),
                Parametro("@ID_AlmacenDestino", transferenciaDto.ID_AlmacenDestino),
                Parametro("@ID_Usuario", transferenciaDto.ID_Usuario),
                Parametro("@Observaciones", transferenciaDto.Observaciones),
                Parametro("@Detalles", Json(transferenciaDto.Detalles)));

            return CreatedAtAction("GetTransferencia", new { id }, (await GetTransferencia(id)).Value);
        }

        // POST: api/Transferencias/5/recibir
        [HttpPost("{id}/recibir")]
        public async Task<IActionResult> RecibirTransferencia(int id, AccionUsuarioRequestDTO accionDto)
        {
            await Finalizar(id, accionDto, true);

            return NoContent();
        }

        // POST: api/Transferencias/5/rechazar  (Motivo es obligatorio)
        [HttpPost("{id}/rechazar")]
        public async Task<IActionResult> RechazarTransferencia(int id, AccionUsuarioRequestDTO accionDto)
        {
            await Finalizar(id, accionDto, false);

            return NoContent();
        }

        private Task<int> Finalizar(int id, AccionUsuarioRequestDTO accionDto, bool recibir)
        {
            return _context.Database.EjecutarAsync("sp_Transferencia_Finalizar",
                Parametro("@ID_Transferencia", id),
                Parametro("@ID_Usuario", accionDto.ID_Usuario),
                Parametro("@Recibir", recibir),
                Parametro("@Motivo", accionDto.Motivo));
        }
    }
}
