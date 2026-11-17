using Aplicacion.Interfaces;
using Dominio.Entidades;
using Dominio.Modelos;
using Infraestructura.Persistencia;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infraestructura.Servicios
{
    public class MenuServicio : IMenuServicio
    {
        private readonly BaseDbContext _context;       
        public MenuServicio(BaseDbContext context)
        {
            _context = context;
        }
        public async Task<List<MenuModelo>> CargarMenu()
        {

            var menus = await _context.Menus.AsNoTracking()
                .Where(m => m.Activo)
                .OrderBy(l => l.Id)
                .ToListAsync();
            return ConstruirArbol(menus);

        }

        public async Task<List<MenuModelo>> CargarMenuPorRol(int rol)
        {
            var rolActivo = await _context.Roles
                    .AnyAsync(r => r.Id == rol && r.Activo);

                if (!rolActivo)
                    return new List<MenuModelo>();

                var menus = await _context.Menus
                    .AsNoTracking()
                    .Where(m => m.Activo)
                    .OrderBy(m => m.Id)
                    .ToListAsync();

                var idsAutorizados = await _context.RolesMenu
                    .Where(rm => rm.RolId == rol && rm.PuedeVer)
                    .Select(rm => rm.MenuId)
                    .ToListAsync();

                var menusPorId = menus.ToDictionary(m => m.Id);
                var idsVisibles = new HashSet<int>(idsAutorizados);

                // Incluir los padres de los menús autorizados.
                foreach (var id in idsAutorizados)
                {
                    if (!menusPorId.TryGetValue(id, out var menu))
                        continue;

                    var padreId = menu.MenuPadreId;

                    while (padreId.HasValue &&
                           menusPorId.TryGetValue(padreId.Value, out var padre))
                    {
                        if (!idsVisibles.Add(padre.Id))
                            break;

                        padreId = padre.MenuPadreId;
                    }
                }

                var menusVisibles = menus
                    .Where(m => idsVisibles.Contains(m.Id))
                    .ToList();

                return ConstruirArbol(menusVisibles);            
        }

        private static List<MenuModelo> ConstruirArbol(
            IEnumerable<MenuEntidad> entidades)
        {
                var elementos = entidades.ToList();

                var modelos = elementos.ToDictionary(
                    m => m.Id,
                    m => new MenuModelo
                    {
                        Nombre = m.Nombre,
                        Url = m.Url,
                        Icono = m.Icono,
                        Nivel = m.Nivel,
                        SubMenu = new List<MenuModelo>()
                    });

                var raices = new List<MenuModelo>();

                foreach (var entidad in elementos)
                {
                    var modelo = modelos[entidad.Id];

                    if (entidad.MenuPadreId.HasValue &&
                        modelos.TryGetValue(
                            entidad.MenuPadreId.Value, out var padre))
                    {
                        padre.SubMenu.Add(modelo);
                    }
                    else
                    {
                        raices.Add(modelo);
                    }
                }

                return raices;           
        }
    }
}
