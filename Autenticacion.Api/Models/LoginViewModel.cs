using System.ComponentModel.DataAnnotations;

namespace Autenticacion.Api.Models;

public sealed class LoginViewModel
{
    [Required(ErrorMessage = "El usuario es obligatorio.")]
    public string UserName { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    public string? ReturnUrl { get; set; }
}