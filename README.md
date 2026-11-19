# ArquitecturaBase Basic

## Base de API para construir soluciones .NET

ArquitecturaBase Basic es un punto de partida de software para proyectos que necesitan una API en .NET que puede desplegarse como servicio independiente organizada por responsabilidades y conectada a SQL Server. El código incluido separa dominio, aplicación, infraestructura y presentación, y ofrece una base concreta sobre la que un equipo puede continuar el desarrollo.

## Qué incluye

- Separación por capas orientada al dominio
- API ASP.NET Core sobre .NET 10.
- Estructura de cuatro proyectos: `Dominio`, `Aplicacion`, `Infraestructura` y `Presentacion`.
- Acceso a datos con Entity Framework Core y migración inicial.
- Consultas de menús y menús filtrados por rol mediante MediatR.
- Despliegue de referencia con Docker Compose y SQL Server.
- Swagger configurable y telemetría OpenTelemetry.
- Script SQL de ejemplo para insertar roles y menús en tablas ya creadas.

## Cómo se despliega

El paquete incluye Docker Compose para levantar la API y SQL Server. El servicio API espera a que SQL Server responda una consulta de salud antes de iniciar. El entorno Basic está configurado para SQL Server; la implementación de PostgreSQL permanece en el código de infraestructura, pero no se incluye como servicio del Compose Basic.

## Alcance funcional visible

La API expone consultas de menús generales y por identificador de rol. El repositorio ofrece entidades de menús y roles, persistencia con Entity Framework Core, documentación OpenAPI y manejo central de excepciones.

## Consideraciones antes de adoptar

ArquitecturaBase Basic es una base técnica, no una aplicación de negocio completa. El paquete no crea la base de datos ni ejecuta migraciones automáticamente al iniciar. La configuración revisada tampoco aplica autenticación a los endpoints de menú. Cada equipo debe preparar la base de datos, definir controles de acceso y completar las funciones específicas de su producto antes de publicar un servicio en un entorno real.

La documentación describe únicamente los componentes presentes en el ZIP revisado; no atribuye módulos, niveles de servicio o certificaciones que el repositorio no demuestra.
