# clean-architecture-cqrs-template-dotnet

Plantilla base para el desarrollo de aplicaciones .NET utilizando Clean Architecture, CQRS con MediatR y Entity Framework Core.

El objetivo de este proyecto es proporcionar una estructura reutilizable para la construcción de APIs empresariales, manteniendo una clara separación de responsabilidades entre dominio, casos de uso, infraestructura y presentación.

Esta solución puede utilizarse como punto de partida para sistemas administrativos, APIs corporativas, microservicios o aplicaciones modulares que requieran una arquitectura mantenible y escalable.

---

## Arquitectura

La solución se encuentra organizada en cuatro proyectos principales:

```text
├─ Dominio
├─ Aplicacion
├─ Infraestructura
└─ Presentacion
```

### Dependencias entre capas

```text
Presentacion
      │
      ▼
 Aplicacion
      │
      ▼
   Dominio

Infraestructura
      ▲
      │
 Aplicacion
```

Las dependencias apuntan hacia el núcleo de negocio siguiendo los principios de Clean Architecture.

### Dominio

Contiene las entidades y reglas principales del negocio. Esta capa es independiente de frameworks, motores de base de datos y tecnologías externas.

### Aplicación

Contiene los casos de uso, contratos, consultas y comandos de la aplicación. Implementa CQRS mediante MediatR para separar operaciones de lectura y escritura.

### Infraestructura

Implementa el acceso a datos mediante Entity Framework Core, la configuración de persistencia y cualquier integración con servicios externos.

### Presentación

Expone los casos de uso mediante ASP.NET Core y configura la composición de dependencias de toda la solución.

---

## Características incluidas

- Clean Architecture.
- CQRS mediante MediatR.
- Inyección de Dependencias.
- ASP.NET Core.
- Entity Framework Core.
- SQL Server.
- Migraciones iniciales.
- Swagger / OpenAPI.
- OpenTelemetry.
- Docker Compose.
- Manejo centralizado de excepciones.
- Separación de responsabilidades por capas.
- Queries y Handlers de referencia.
- Commands y Handlers de referencia.

---

## Implementación CQRS

El proyecto incluye una implementación de referencia basada en MediatR para separar las operaciones de lectura y escritura.

### Query de ejemplo

```csharp
public record ObtenerMenuQuery()
    : IRequest<List<MenuModelo>>;
```

### Handler de ejemplo

```csharp
public class MenuHandler
    : IRequestHandler<ObtenerMenuQuery, List<MenuModelo>>
{
    private readonly IMenuServicio _menuServicio;

    public MenuHandler(IMenuServicio menuServicio)
    {
        _menuServicio = menuServicio;
    }

    public async Task<List<MenuModelo>> Handle(
        ObtenerMenuQuery request,
        CancellationToken cancellationToken)
    {
        return await _menuServicio.CargarMenu();
    }
}
```

### Command de ejemplo

```csharp
public record CrearMenuCommand(
    string Nombre,
    string Url,
    string Icono,
    string Nivel,
    int PadreId
) : IRequest<bool>;
```

### Flujo de ejecución

```text
Controller / Endpoint
         │
         ▼
 Query o Command
         │
         ▼
      Handler
         │
         ▼
      Servicio
         │
         ▼
   Infraestructura
         │
         ▼
    SQL Server
```

---

## Estructura del proyecto

```text
ArquitecturaBaseBasic
│
├── Dominio
│   ├── Entidades
│   ├── Modelos
│   └── Enumeraciones
│
├── Aplicacion
│   ├── Interfaces
│   ├── Commands
│   ├── Queries
│   ├── Handlers
│   └── Configuracion
│
├── Infraestructura
│   ├── Persistencia
│   ├── Servicios
│   ├── Repositorios
│   └── Migraciones
│
└── Presentacion
    ├── Controllers
    ├── Middleware
    └── Configuracion
```

---

## Tecnologías utilizadas

| Tecnología | Descripción |
|------------|-------------|
| .NET 10 | Plataforma principal |
| ASP.NET Core | Exposición de la API |
| MediatR | Implementación de CQRS |
| Entity Framework Core | Acceso a datos |
| SQL Server | Persistencia |
| Docker | Contenerización |
| Swagger/OpenAPI | Documentación de endpoints |
| OpenTelemetry | Observabilidad y telemetría |

---

## Despliegue rápido

La solución incluye configuración mediante Docker Compose para levantar:

- API ASP.NET Core.
- SQL Server.

La API espera la disponibilidad de SQL Server antes de iniciar la aplicación.

### Levantar el entorno

```bash
docker compose up -d
```

---

## Alcance de la plantilla

ArquitecturaBase Basic no pretende ser una aplicación de negocio completa.

Su objetivo es proporcionar una base técnica para iniciar nuevos proyectos con una arquitectura moderna y preparada para crecer.

La solución proporciona:

- Separación de capas.
- Inversión de dependencias.
- Implementación de CQRS.
- Persistencia mediante Entity Framework Core.
- Inyección de dependencias.
- Observabilidad mediante OpenTelemetry.
- Contenerización mediante Docker.

Cada equipo puede extender esta base agregando:

- Autenticación y autorización.
- Nuevos módulos de negocio.
- Integraciones externas.
- Eventos de dominio.
- Validaciones avanzadas.
- Arquitectura basada en eventos.

---

## Casos de uso recomendados

- APIs empresariales.
- Sistemas administrativos.
- ERP internos.
- Microservicios .NET.
- Sistemas modulares.
- Aplicaciones con Clean Architecture.
- Soluciones basadas en CQRS y MediatR.
- Proyectos que requieran mantenibilidad y escalabilidad desde etapas tempranas.

---

## Objetivo del repositorio

Demostrar la implementación de una arquitectura empresarial moderna basada en:

- Clean Architecture.
- CQRS.
- MediatR.
- Entity Framework Core.
- SQL Server.
- Docker.
- Buenas prácticas de desarrollo en .NET.

Esta plantilla sirve como base para nuevos proyectos y como referencia arquitectónica para desarrolladores que deseen construir aplicaciones desacopladas, mantenibles y orientadas a casos de uso.
