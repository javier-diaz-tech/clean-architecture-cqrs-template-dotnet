using Autenticacion.Api.Models;
using Autenticacion.Api.Persistencia;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Autenticacion.Api.Controllers;

public sealed class AccountController : Controller
{
    private readonly SignInManager<AplicacionUsuario> _signInManager;

    public AccountController(
        SignInManager<AplicacionUsuario> signInManager)
    {
        _signInManager = signInManager;
    }

    [HttpGet("/Account/Login")]
    public IActionResult Login(string? returnUrl)
    {
        return View(new LoginViewModel
        {
            ReturnUrl = returnUrl
        });
    }

    [HttpPost("/Account/Login")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _signInManager.PasswordSignInAsync(
            model.UserName,
            model.Password,
            isPersistent: false,
            lockoutOnFailure: true);

        if (result.Succeeded)
        {
            if (!string.IsNullOrWhiteSpace(model.ReturnUrl) &&
                Url.IsLocalUrl(model.ReturnUrl))
            {
                return Redirect(model.ReturnUrl);
            }

            return Redirect("/");
        }

        ModelState.AddModelError(
            string.Empty,
            "Usuario o contraseña incorrectos.");

        return View(model);
    }
}