using Autenticacion.Api.Persistencia;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Autenticacion.Api.Dominio;
namespace Autenticacion.Api.Controllers;

[ApiController]
[Route("api/administracion/usuarios")]
[Authorize(Roles = "Administrador")]
public sealed class AdministracionUsuariosController : ControllerBase
{
    private readonly UserManager<AplicacionUsuario> _users;
    private readonly RoleManager<IdentityRole> _roles;

    public AdministracionUsuariosController(
        UserManager<AplicacionUsuario> users,
        RoleManager<IdentityRole> roles)
    {
        _users = users;
        _roles = roles;
    }  

    [HttpPost]
    public async Task<IActionResult> CrearUsuario(
        [FromBody] CrearUsuarioRequest entrada)
    {
        var user = new AplicacionUsuario
        {
            UserName = entrada.UserName.Trim(),
            Email = entrada.Email.Trim(),
            EmailConfirmed = false,
            Activo = true
        };

        var result = await _users.CreateAsync(user, entrada.Password);

        if (!result.Succeeded)
        {
            return BadRequest(new
            {
                errores = result.Errors.Select(e => new
                {
                    e.Code,
                    e.Description
                })
            });
        }

        return Created(
            $"/api/administracion/usuarios/{user.Id}",
            new { user.Id, user.UserName, user.Email, user.Activo });
    }

    [HttpPost("{userId}/roles")]
    public async Task<IActionResult> AsignarRol(
        string userId,
        [FromBody] AsignarRolRequest entrada)
    {
        var user = await _users.FindByIdAsync(userId);

        if (user is null)
            return NotFound(new { mensaje = "Usuario no encontrado." });

        var roleName = entrada.Rol.Trim();

        if (!await _roles.RoleExistsAsync(roleName))
            return BadRequest(new { mensaje = "El rol no existe." });

        var result = await _users.AddToRoleAsync(user, roleName);

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return NoContent();
    }

    [HttpPut("{userId}/estado")]
    public async Task<IActionResult> CambiarEstado(
        string userId,
        [FromQuery] bool activo)
    {
        var user = await _users.FindByIdAsync(userId);

        if (user is null)
            return NotFound();

        user.Activo = activo;

        var result = await _users.UpdateAsync(user);

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return NoContent();
    }
}