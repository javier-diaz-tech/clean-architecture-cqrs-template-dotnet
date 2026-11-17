using MediatR;
using Dominio.Modelos;
using Aplicacion.Interfaces;
using Microsoft.Extensions.Logging;
namespace Aplicacion.Menu
{

    public class MenuHandler : IRequestHandler<ObtenerMenuQuery, List<MenuModelo>>
    {

        IMenuServicio _menuServicio;       
        public MenuHandler(IMenuServicio menuServicio)
        {
            _menuServicio = menuServicio;
        }

     

        public async Task<List<MenuModelo>> Handle(ObtenerMenuQuery request, CancellationToken cancellationToken)
        {
           return await _menuServicio.CargarMenu();           
        }
    }
}
