using Microsoft.Extensions.DependencyInjection;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Autenticacion.Api.Persistencia;

public sealed class InicializadorOpenIddict : IHostedService
{
    private readonly IServiceProvider _services;
    private readonly IConfiguration _configuration;

    public InicializadorOpenIddict(
        IServiceProvider services,
        IConfiguration configuration)
    {
        _services = services;
        _configuration = configuration;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await using var scope = _services.CreateAsyncScope();

        var manager = scope.ServiceProvider
            .GetRequiredService<IOpenIddictApplicationManager>();

        var clientId = _configuration["OpenIddict:ClientId"]
            ?? throw new InvalidOperationException(
                "Falta OpenIddict:ClientId.");

        var redirectUri = _configuration["OpenIddict:RedirectUri"]
            ?? throw new InvalidOperationException(
                "Falta OpenIddict:RedirectUri.");

        if (await manager.FindByClientIdAsync(
                clientId, cancellationToken) is not null)
        {
            return;
        }

        var descriptor = new OpenIddictApplicationDescriptor
        {
            ClientId = clientId,
            DisplayName = "ArquitecturaBase Web",
            ClientType = ClientTypes.Public
        };

        descriptor.RedirectUris.Add(new Uri(redirectUri));

        descriptor.Permissions.Add(Permissions.Endpoints.Authorization);
        descriptor.Permissions.Add(Permissions.Endpoints.Token);

        descriptor.Permissions.Add(Permissions.GrantTypes.AuthorizationCode);
        descriptor.Permissions.Add(Permissions.GrantTypes.RefreshToken);
        descriptor.Permissions.Add(Permissions.ResponseTypes.Code);

        descriptor.Permissions.Add(Permissions.Scopes.Profile);
        descriptor.Permissions.Add(Permissions.Scopes.Email);
        descriptor.Permissions.Add(Permissions.Scopes.Roles);
        descriptor.Permissions.Add(Permissions.Prefixes.Scope + "arquitecturabase.api");

        await manager.CreateAsync(descriptor, cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
        => Task.CompletedTask;
}