using Microsoft.AspNetCore.Mvc;
using WebAPI.Contexts;
using WebAPI.DTOs;
using WebAPI.Helpers;
using static WebAPI.Helpers.SpHelper;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RolesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Roles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RolResponseDTO>>> GetRoles()
        {
            var roles = await _context.Database.ConsultarAsync<RolResponseDTO>("sp_Rol_Listar");

            // Un solo SP trae los permisos de todos los roles y aquí se reparten
            var permisos = await _context.Database.ConsultarAsync<RolPermisoDTO>("sp_Rol_Permisos");

            foreach (var rol in roles)
            {
                rol.Permisos = permisos.Where(p => p.ID_Rol == rol.ID_Rol)
                    .Select(p => new PermisoSimpleDTO { ID_Permiso = p.ID_Permiso, Codigo = p.Codigo, Nombre = p.Nombre })
                    .ToList();
            }

            return roles;
        }

        // GET: api/Roles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RolResponseDTO>> GetRol(int id)
        {
            var rol = (await _context.Database.ConsultarAsync<RolResponseDTO>("sp_Rol_Obtener",
                Parametro("@ID_Rol", id))).FirstOrDefault();

            if (rol == null)
            {
                return NotFound();
            }

            var permisos = await _context.Database.ConsultarAsync<RolPermisoDTO>("sp_Rol_Permisos", Parametro("@ID_Rol", id));

            rol.Permisos = permisos
                .Select(p => new PermisoSimpleDTO { ID_Permiso = p.ID_Permiso, Codigo = p.Codigo, Nombre = p.Nombre })
                .ToList();

            return rol;
        }

        // POST: api/Roles
        [HttpPost]
        public async Task<ActionResult<RolResponseDTO>> PostRol(RolRequestDTO rolDto)
        {
            var id = await _context.Database.ObtenerIdAsync("sp_Rol_Insertar",
                Parametro("@NombreRol", rolDto.NombreRol),
                Parametro("@Descripcion", rolDto.Descripcion),
                Parametro("@Permisos", Json(rolDto.Permisos)));

            return CreatedAtAction("GetRol", new { id }, (await GetRol(id)).Value);
        }

        // PUT: api/Roles/5  (también reemplaza los permisos asignados)
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRol(int id, RolRequestDTO rolDto)
        {
            await _context.Database.EjecutarAsync("sp_Rol_Actualizar",
                Parametro("@ID_Rol", id),
                Parametro("@NombreRol", rolDto.NombreRol),
                Parametro("@Descripcion", rolDto.Descripcion),
                Parametro("@Permisos", Json(rolDto.Permisos)));

            return NoContent();
        }

        // DELETE: api/Roles/5  (borrado lógico; no se permite si tiene usuarios activos)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRol(int id)
        {
            await _context.Database.EjecutarAsync("sp_Rol_CambiarEstado",
                Parametro("@ID_Rol", id), Parametro("@Estado", false));

            return NoContent();
        }

        // PATCH: api/Roles/5/activar
        [HttpPatch("{id}/activar")]
        public async Task<IActionResult> ActivarRol(int id)
        {
            await _context.Database.EjecutarAsync("sp_Rol_CambiarEstado",
                Parametro("@ID_Rol", id), Parametro("@Estado", true));

            return NoContent();
        }
    }
}
