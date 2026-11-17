namespace Autenticacion.Api.Dominio
{
    public sealed record CrearUsuarioRequest(
        string UserName,
        string Email,
        string Password);

    public sealed record AsignarRolRequest(string Rol);
}
