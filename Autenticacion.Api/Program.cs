using Autenticacion.Api.Inicializacion;
using Autenticacion.Api.Persistencia;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using OpenIddict.Abstractions;
using Swashbuckle.AspNetCore.SwaggerGen;


var builder = WebApplication.CreateBuilder(args);

var provider = builder.Configuration["Persistence:Provider"]
    ?? "SqlServer";

var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "Falta ConnectionStrings:DefaultConnection.");

builder.Services.AddDbContext<IdentidadDbContext>(options =>
{
    if (provider.Equals(
        "SqlServer", StringComparison.OrdinalIgnoreCase))
    {
        options.UseSqlServer(connectionString);
    }
    else if (provider.Equals(
        "PostgreSql", StringComparison.OrdinalIgnoreCase))
    {
        options.UseNpgsql(connectionString);
    }
    else
    {
        throw new InvalidOperationException(
            $"Proveedor no soportado: {provider}");
    }

    options.UseOpenIddict();
});

builder.Services
    .AddIdentity<AplicacionUsuario,IdentityRole>(options =>
    {
        options.User.RequireUniqueEmail = true;
        options.Password.RequiredLength = 12;
        options.Password.RequireDigit = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.DefaultLockoutTimeSpan =
            TimeSpan.FromMinutes(15);
        options.Lockout.AllowedForNewUsers = true;
    })
    .AddEntityFrameworkStores<IdentidadDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddOpenIddict()
    .AddCore(options =>
    {
        // Configurar OpenIddict para usar las entidades y almacenes de EF Core
        options.UseEntityFrameworkCore()
               .UseDbContext<IdentidadDbContext>();
    })
    .AddServer(options =>
    {
        var issuer = builder.Configuration["OpenIddict:Issuer"]
            ?? throw new InvalidOperationException(
                "Falta OpenIddict:Issuer.");
        options.SetIssuer(new Uri(issuer));
        options.SetAuthorizationEndpointUris("/connect/authorize")
               .SetTokenEndpointUris("/connect/token");

        options.AllowAuthorizationCodeFlow()
               .AllowRefreshTokenFlow();

        options.RegisterScopes(
            OpenIddictConstants.Scopes.OpenId,
            OpenIddictConstants.Scopes.Profile,
            OpenIddictConstants.Scopes.Email,
            OpenIddictConstants.Scopes.Roles,
            "arquitecturabase.api");

        // Solo para desarrollo local.
        options.AddDevelopmentEncryptionCertificate()
               .AddDevelopmentSigningCertificate();

        options.DisableAccessTokenEncryption();

        options.SetAccessTokenLifetime(TimeSpan.FromMinutes(10));
             //  .SetRefreshTokenLifetime(TimeSpan.FromDays(7));

        options.UseAspNetCore()
               .EnableAuthorizationEndpointPassthrough();
    });

// Add services to the container.
builder.Services.AddControllersWithViews();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
builder.Services.AddHostedService<InicializadorOpenIddict>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Inicializar roles y administrador
await InicializadorIdentidad.InicializarAsync(
    app.Services,
    app.Configuration);

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapDefaultControllerRoute();

app.Run();
