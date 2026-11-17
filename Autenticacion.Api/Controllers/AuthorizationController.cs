using Autenticacion.Api.Persistencia;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using System.Security.Claims;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Autenticacion.Api.Controllers;

public sealed class AuthorizationController : Controller
{
    private readonly UserManager<AplicacionUsuario> _userManager;

    public AuthorizationController(
        UserManager<AplicacionUsuario> userManager)
    {
        _userManager = userManager;
    }

    [AllowAnonymous]
    [HttpGet("~/connect/authorize")]
    [HttpPost("~/connect/authorize")]
    public async Task<IActionResult> AuthorizeEndpoint()
    {
        var request = HttpContext.GetOpenIddictServerRequest()
            ?? throw new InvalidOperationException(
                "No se pudo recuperar la solicitud OpenID Connect.");

        // La cookie de Identity representa la sesión del usuario.
        var authentication = await HttpContext.AuthenticateAsync(
            IdentityConstants.ApplicationScheme);

        if (!authentication.Succeeded)
        {
            var returnUrl = Request.PathBase +
                            Request.Path +
                            Request.QueryString;

            return RedirectToAction(
                "Login", "Account",
                new { returnUrl });
        }

        var subject = authentication.Principal?
            .FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrWhiteSpace(subject))
        {
            return Forbid(
                OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        var user = await _userManager.FindByIdAsync(subject);

        if (user is null || !user.Activo)
        {
            await HttpContext.SignOutAsync(
                IdentityConstants.ApplicationScheme);

            return Forbid(
                OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        var roles = await _userManager.GetRolesAsync(user);

        var identity = new ClaimsIdentity(
            OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
            Claims.Name,
            Claims.Role);

        identity.AddClaim(new Claim(Claims.Subject, user.Id));

        if (!string.IsNullOrWhiteSpace(user.UserName))
            identity.AddClaim(new Claim(Claims.Name, user.UserName));

        if (!string.IsNullOrWhiteSpace(user.Email))
            identity.AddClaim(new Claim(Claims.Email, user.Email));

        foreach (var role in roles)
            identity.AddClaim(new Claim(Claims.Role, role));

        var principal = new ClaimsPrincipal(identity);

        // OpenIddict solo debe conceder los scopes que el cliente
        // haya solicitado y que el servidor permita.
        principal.SetScopes(request.GetScopes());

        // Identifica la audiencia lógica de la API que consumirá el token.
        principal.SetResources("arquitecturabase.api");

        // Define en qué tipo de token se exponen las claims.
        principal.SetDestinations(claim => claim.Type switch
        {
            Claims.Subject =>
                new[] { Destinations.AccessToken, Destinations.IdentityToken },

            Claims.Name =>
                new[] { Destinations.AccessToken, Destinations.IdentityToken },

            Claims.Email =>
                new[] { Destinations.AccessToken, Destinations.IdentityToken },

            Claims.Role =>
                new[] { Destinations.AccessToken },

            _ => Array.Empty<string>()
        });

        return SignIn(
            principal,
            OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }
}