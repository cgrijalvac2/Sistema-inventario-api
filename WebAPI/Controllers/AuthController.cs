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
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: api/Auth/login
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDTO>> Login(LoginRequestDTO loginDto)
        {
            var datos = (await _context.Database.ConsultarAsync<AuthUsuarioDTO>("sp_Auth_ObtenerUsuario",
                Parametro("@CorreoElectronico", loginDto.CorreoElectronico))).FirstOrDefault();

            // La contraseña se valida en C# contra el hash guardado (PasswordHasher)
            var usuario = new Usuario { PasswordHash = datos?.PasswordHash ?? string.Empty };

            // Mismo mensaje si no existe el correo o si la contraseña está mal (no se le dice al atacante cuál falló)
            if (datos == null || !HashUsuario.ValidatePassword(usuario, loginDto.Password))
            {
                return Unauthorized(new { mensaje = "Correo o contraseña incorrectos" });
            }

            if (!datos.Estado || !datos.RolEstado)
            {
                return Unauthorized(new { mensaje = "El usuario está inactivo" });
            }

            await _context.Database.EjecutarAsync("sp_Auth_RegistrarAcceso", Parametro("@ID_Usuario", datos.ID_Usuario));

            var permisos = await _context.Database.ConsultarAsync<PermisoSimpleDTO>("sp_Auth_Permisos",
                Parametro("@ID_Usuario", datos.ID_Usuario));

            var almacenes = await _context.Database.ConsultarAsync<AlmacenSimpleDTO>("sp_Auth_Almacenes",
                Parametro("@ID_Usuario", datos.ID_Usuario));

            return new LoginResponseDTO()
            {
                ID_Usuario = datos.ID_Usuario,
                NombreCompleto = datos.NombreCompleto,
                CorreoElectronico = datos.CorreoElectronico,
                ID_Rol = datos.ID_Rol,
                NombreRol = datos.NombreRol,
                Permisos = permisos.Select(p => p.Codigo).ToList(),
                Almacenes = almacenes
            };
        }
    }
}
