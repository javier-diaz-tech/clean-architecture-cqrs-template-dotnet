using Autenticacion.Api.Persistencia;
using Microsoft.AspNetCore.Identity;

namespace Autenticacion.Api.Inicializacion;

public static class InicializadorIdentidad
{
    public static async Task InicializarAsync(
        IServiceProvider serviceProvider,
        IConfiguration configuration)
    {
        using var scope = serviceProvider.CreateScope();

        var roleManager = scope.ServiceProvider
            .GetRequiredService<RoleManager<IdentityRole>>();

        var userManager = scope.ServiceProvider
            .GetRequiredService<UserManager<AplicacionUsuario>>();

        // 1. Crear roles iniciales
        string[] roles =
        {
            "Administrador",
            "Usuario"
        };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                var result = await roleManager.CreateAsync(
                    new IdentityRole(role));

                if (!result.Succeeded)
                {
                    throw new Exception(
                        $"Error creando el rol {role}: " +
                        string.Join(", ",
                            result.Errors.Select(e => e.Description)));
                }
            }
        }

        // 2. Obtener datos del administrador inicial
        var email = configuration["InitialAdmin:Email"];
        var password = configuration["InitialAdmin:Password"];
        var username = configuration["InitialAdmin:UserName"];

        if (string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(password) ||
            string.IsNullOrWhiteSpace(username))
        {
            throw new InvalidOperationException(
                "Falta configurar InitialAdmin:Email, UserName o Password.");
        }

        // 3. Comprobar si ya existe
        var user = await userManager.FindByEmailAsync(email);

        if (user is null)
        {
            user = new AplicacionUsuario
            {
                UserName = username,
                Email = email,
                EmailConfirmed = true,
                Activo = true
            };

            var result = await userManager.CreateAsync(
                user, password);

            if (!result.Succeeded)
            {
                throw new Exception(
                    "Error creando el administrador: " +
                    string.Join(", ",
                        result.Errors.Select(e => e.Description)));
            }
        }

        // 4. Asignar rol de administrador
        if (!await userManager.IsInRoleAsync(
                user, "Administrador"))
        {
            var result = await userManager.AddToRoleAsync(
                user, "Administrador");

            if (!result.Succeeded)
            {
                throw new Exception(
                    "Error asignando el rol: " +
                    string.Join(", ",
                        result.Errors.Select(e => e.Description)));
            }
        }
    }
}