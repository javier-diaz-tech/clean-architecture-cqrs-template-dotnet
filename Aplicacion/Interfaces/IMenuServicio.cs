using Dominio.Modelos;

namespace Aplicacion.Interfaces
{
    public interface IMenuServicio
    {
        public Task<List<MenuModelo>> CargarMenu();

        public Task<List<MenuModelo>> CargarMenuPorRol(int rol);       
        
    }
}
