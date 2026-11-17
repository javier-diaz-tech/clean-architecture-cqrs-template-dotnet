using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion
{

    public static class DependencyInjection
    {
        public static IServiceCollection AddAplicacionServicios(this IServiceCollection services)
        {
            // MediatR busca automáticamente todos los Handlers dentro de este proyecto de Aplicación
            services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly)
            );

            return services;
        }
    }
}
