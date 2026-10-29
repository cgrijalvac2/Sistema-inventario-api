using Microsoft.AspNetCore.Identity;
using WebAPI.Models;

namespace WebAPI.Helpers
{
    public class HashUsuario
    {
        public static string HashPassword(Usuario usuario, string password)
        {
            var hasher = new PasswordHasher<Usuario>();

            var hash = hasher.HashPassword(usuario, password);

            return hash;
        }

        public static bool ValidatePassword(Usuario usuario, string passwordIngresado)
        {
            var hasher = new PasswordHasher<Usuario>();

            var resultado = hasher.VerifyHashedPassword(usuario, usuario.PasswordHash, passwordIngresado);

            return resultado != PasswordVerificationResult.Failed;
        }
    }
}
