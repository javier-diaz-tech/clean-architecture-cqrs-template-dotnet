using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Autenticacion.Api.Persistencia
{
    public sealed class AplicacionUsuario : IdentityUser
    {
        public bool Activo { get; set; } = true;
    }

    public sealed class IdentidadDbContext
        : IdentityDbContext<AplicacionUsuario, IdentityRole, string>
    {
        public IdentidadDbContext(
            DbContextOptions<IdentidadDbContext> options)
            : base(options)
        {
        }

        public async Task<AplicacionUsuario> ValidarUsuarioAsync(string usuario, string contrasena)
        {
            var user = await Users.SingleOrDefaultAsync(u => u.UserName == usuario);
            var passwordHasher = new PasswordHasher<AplicacionUsuario>();
            var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, contrasena);
            return result == PasswordVerificationResult.Success ? user : new AplicacionUsuario();
        }
    }
}
