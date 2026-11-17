using MediatR;
using Dominio.Modelos;
namespace Aplicacion.Menu
{
    public record ObtenerMenuQuery() : IRequest<List<MenuModelo>>;
}
