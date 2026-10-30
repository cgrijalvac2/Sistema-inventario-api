using Microsoft.AspNetCore.Mvc;
using WebAPI.Contexts;
using WebAPI.DTOs;
using WebAPI.Helpers;
using static WebAPI.Helpers.SpHelper;

namespace WebAPI.Controllers
{
    // Mantenimiento de la tabla RolPermiso: a qué permisos tiene acceso cada rol
    [Route("api/[controller]")]
    [ApiController]
    public class RolPermisoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RolPermisoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/RolPermiso?idRol=2   (sin idRol = las asignaciones de todos los roles)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RolPermisoDTO>>> GetRolPermisos(int? idRol)
        {
            return await _context.Database.ConsultarAsync<RolPermisoDTO>("sp_Rol_Permisos",
                Parametro("@ID_Rol", idRol));
        }

        // POST: api/RolPermiso   { "ID_Rol": 2, "ID_Permiso": 5 }
        [HttpPost]
        public async Task<IActionResult> PostRolPermiso(RolPermisoRequestDTO rolPermisoDto)
        {
            await _context.Database.EjecutarAsync("sp_RolPermiso_Asignar",
                Parametro("@ID_Rol", rolPermisoDto.ID_Rol),
                Parametro("@ID_Permiso", rolPermisoDto.ID_Permiso));

            return NoContent();
        }

        // DELETE: api/RolPermiso/2/5   (idRol / idPermiso)
        [HttpDelete("{idRol}/{idPermiso}")]
        public async Task<IActionResult> DeleteRolPermiso(int idRol, int idPermiso)
        {
            await _context.Database.EjecutarAsync("sp_RolPermiso_Quitar",
                Parametro("@ID_Rol", idRol),
                Parametro("@ID_Permiso", idPermiso));

            return NoContent();
        }
    }
}
