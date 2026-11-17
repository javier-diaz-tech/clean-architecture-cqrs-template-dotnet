using MediatR;
using Dominio.Modelos;
namespace Aplicacion.Menu
{
    public record ObtenerMenuPorRolQuery(int Rol) : IRequest<List<MenuModelo>>;
}
