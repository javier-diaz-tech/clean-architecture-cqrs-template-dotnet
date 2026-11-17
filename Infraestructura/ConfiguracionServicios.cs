using Aplicacion.Interfaces;
using Infraestructura.Persistencia;
using Infraestructura.Servicios;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infraestructura
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfraestructuraServicios(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            var provider = configuration["Persistence:Provider"]
            ?? "SqlServer";

            var connectionString =
                configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "No se configuró ConnectionStrings:DefaultConnection.");

            serviceCollection.AddDbContext<BaseDbContext>(options =>
            {
                switch (provider.ToLowerInvariant())
                {
                    case "sqlserver":
                        options.UseSqlServer(connectionString);
                        break;

                    case "postgresql":
                        options.UseNpgsql(connectionString);
                        break;

                    default:
                        throw new InvalidOperationException(
                            $"Proveedor de base de datos no soportado: {provider}");
                }
            });
            // Configurar servicios aquí
            serviceCollection.AddScoped<IMenuServicio, MenuServicio>();
        
            return serviceCollection;
        }

    }
}
