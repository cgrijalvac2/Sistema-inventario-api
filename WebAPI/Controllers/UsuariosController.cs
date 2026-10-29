using Microsoft.AspNetCore.Mvc;
using WebAPI.Contexts;
using WebAPI.DTOs;
using WebAPI.Helpers;
using WebAPI.Models;
using static WebAPI.Helpers.SpHelper;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly ApplicationDbContext _dbcontext;

        public UsuariosController(ApplicationDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        // GET: api/Usuarios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UsuarioResponseDTO>>> GetUsuarios()
        {
            var usuarios = await _dbcontext.Database.ConsultarAsync<UsuarioResponseDTO>("sp_Usuario_Listar");

            // Un solo SP trae los almacenes de todos los usuarios y aquí se reparten
            var almacenes = await _dbcontext.Database.ConsultarAsync<UsuarioAlmacenDTO>("sp_Usuario_Almacenes");

            foreach (var usuario in usuarios)
            {
                usuario.Almacenes = almacenes.Where(a => a.ID_Usuario == usuario.ID_Usuario)
                    .Select(a => new AlmacenSimpleDTO { ID_Almacen = a.ID_Almacen, Codigo = a.Codigo, Nombre = a.Nombre })
                    .ToList();
            }

            return usuarios;
        }

        // GET: api/Usuarios/5
        [HttpGet("{usuarioid}")]
        public async Task<ActionResult<UsuarioResponseDTO>> GetUsuario(int usuarioid)
        {
            var usuario = (await _dbcontext.Database.ConsultarAsync<UsuarioResponseDTO>("sp_Usuario_Obtener",
                Parametro("@ID_Usuario", usuarioid))).FirstOrDefault();

            if (usuario == null)
            {
                return NotFound();
            }

            var almacenes = await _dbcontext.Database.ConsultarAsync<UsuarioAlmacenDTO>("sp_Usuario_Almacenes",
                Parametro("@ID_Usuario", usuarioid));

            usuario.Almacenes = almacenes
                .Select(a => new AlmacenSimpleDTO { ID_Almacen = a.ID_Almacen, Codigo = a.Codigo, Nombre = a.Nombre })
                .ToList();

            return usuario;
        }

        // POST: api/Usuarios
        [HttpPost]
        public async Task<ActionResult<UsuarioResponseDTO>> PostUsuario(UsuarioRequestDTO usuarioDto)
        {
            if (string.IsNullOrWhiteSpace(usuarioDto.Password))
            {
                return BadRequest(new { mensaje = "La contraseña es requerida" });
            }

            // La contraseña se convierte en hash aquí; a la base de datos solo llega el hash
            var hash = HashUsuario.HashPassword(new Usuario(), usuarioDto.Password);

            var id = await _dbcontext.Database.ObtenerIdAsync("sp_Usuario_Insertar",
                Parametro("@NombreCompleto", usuarioDto.NombreCompleto),
                Parametro("@CorreoElectronico", usuarioDto.CorreoElectronico),
                Parametro("@PasswordHash", hash),
                Parametro("@ID_Rol", usuarioDto.ID_Rol),
                Parametro("@Almacenes", Json(usuarioDto.Almacenes)));

            return CreatedAtAction("GetUsuario", new { usuarioid = id }, (await GetUsuario(id)).Value);
        }

        // PUT: api/Usuarios/5  (si Password viene vacío se conserva la contraseña actual)
        [HttpPut("{usuarioid}")]
        public async Task<IActionResult> PutUsuario(int usuarioid, UsuarioRequestDTO usuarioDto)
        {
            string? hash = null;
            if (!string.IsNullOrWhiteSpace(usuarioDto.Password))
            {
                hash = HashUsuario.HashPassword(new Usuario(), usuarioDto.Password);
            }

            await _dbcontext.Database.EjecutarAsync("sp_Usuario_Actualizar",
                Parametro("@ID_Usuario", usuarioid),
                Parametro("@NombreCompleto", usuarioDto.NombreCompleto),
                Parametro("@CorreoElectronico", usuarioDto.CorreoElectronico),
                Parametro("@PasswordHash", hash),
                Parametro("@ID_Rol", usuarioDto.ID_Rol),
                Parametro("@Almacenes", Json(usuarioDto.Almacenes)));

            return NoContent();
        }

        // DELETE: api/Usuarios/5  (borrado lógico: el usuario ya no puede iniciar sesión)
        [HttpDelete("{usuarioid}")]
        public async Task<IActionResult> DeleteUsuario(int usuarioid)
        {
            await _dbcontext.Database.EjecutarAsync("sp_Usuario_CambiarEstado",
                Parametro("@ID_Usuario", usuarioid), Parametro("@Estado", false));

            return NoContent();
        }

        // PATCH: api/Usuarios/5/activar
        [HttpPatch("{usuarioid}/activar")]
        public async Task<IActionResult> ActivarUsuario(int usuarioid)
        {
            await _dbcontext.Database.EjecutarAsync("sp_Usuario_CambiarEstado",
                Parametro("@ID_Usuario", usuarioid), Parametro("@Estado", true));

            return NoContent();
        }
    }
}
