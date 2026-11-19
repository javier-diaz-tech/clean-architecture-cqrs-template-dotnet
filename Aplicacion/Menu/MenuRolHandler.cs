using MediatR;
using Dominio.Modelos;
using Aplicacion.Interfaces;
namespace Aplicacion.Menu
{

    public class MenuPorRolHandler : IRequestHandler<ObtenerMenuPorRolQuery, List<MenuModelo>>
    {
        IMenuServicio _menuServicio;        
        public MenuPorRolHandler(IMenuServicio menuServicio)
        {
            _menuServicio = menuServicio;
        }

        public async Task<List<MenuModelo>> Handle(ObtenerMenuPorRolQuery request, CancellationToken cancellationToken)
        {
           return await _menuServicio.CargarMenuPorRol(request.Rol);           
        }
    }
}
