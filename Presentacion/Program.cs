using Aplicacion;
using Infraestructura;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Microsoft.Win32;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Presentacion.ManejadorExcepciones;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//builder.Services.AddOpenIddict()
//    .AddValidation(options =>
//    {
//        options.SetIssuer("https://localhost:5068/");
//        options.AddAudiences("arquitecturabase.api");

//        options.UseSystemNetHttp();
//        options.UseAspNetCore();
//    });


builder.Services.AddInfraestructuraServicios(builder.Configuration);
builder.Services.AddAplicacionServicios();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();


builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();


// 1. Configurar Trazas y Métricas
builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService(builder.Configuration["OpenTelemetry:ServiceName"]))
    .WithTracing(tracing =>
    {
        tracing
        .AddAspNetCoreInstrumentation() // Trazas de peticiones entrantes
        .AddHttpClientInstrumentation(); // Trazas de peticiones salientes (HttpClient)
        if (builder.Configuration.GetValue<bool>("OpenTelemetry:EnableConsoleExporter"))
        {
            tracing.AddConsoleExporter();
        }
        if (builder.Configuration.GetValue<bool>("OpenTelemetry:EnableOtlpExporter"))
        {
            tracing.AddOtlpExporter();             // Enviar a Jaeger, Grafana Tempo, Aspire, etc.
        }
    })
    .WithMetrics(metrics => { metrics
        .AddAspNetCoreInstrumentation() // Métricas de Kestrel y rutas
        .AddHttpClientInstrumentation()
        .AddRuntimeInstrumentation(); // Métricas del runtime de .NET
        if (builder.Configuration.GetValue<bool>("OpenTelemetry:EnableConsoleExporter"))
        {
            metrics.AddConsoleExporter();
        }
        if (builder.Configuration.GetValue<bool>("OpenTelemetry:EnableOtlpExporter"))
        {
            metrics.AddOtlpExporter();
        }
    });

// 2. Integrar Logs estructurados con ILogger
builder.Logging.AddOpenTelemetry(logging =>
{
    logging.IncludeFormattedMessage = true;
    logging.IncludeScopes = true;
    if(builder.Configuration.GetValue<bool>("OpenTelemetry:EnableConsoleExporter"))
    {
        logging.AddConsoleExporter();
    }
    if (builder.Configuration.GetValue<bool>("OpenTelemetry:EnableOtlpExporter"))
    {
        logging.AddOtlpExporter();
    }  
});


var app = builder.Build();

// Swagger configurable independientemente del entorno.
var swaggerEnabled = builder.Configuration.GetValue<bool>("Swagger:Enabled");

if (swaggerEnabled)
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();
app.UseHttpsRedirection();

//app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapGet("/", (ILogger<Program> logger) =>
{
    logger.LogInformation("Petición recibida con correlación automática de TraceId");
    return "OpenTelemetry activo";
});

app.Run();
