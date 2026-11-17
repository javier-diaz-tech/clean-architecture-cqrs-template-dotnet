using Aplicacion.Excepciones;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Presentacion.ManejadorExcepciones
{   

    public sealed class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(
            ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext context,
            Exception exception,
            CancellationToken cancellationToken)
        {
            var (status, title, detail, errors) = exception switch
            {
                ValidationException ex => (
                    StatusCodes.Status400BadRequest,
                    "Error de validación",
                    ex.Message,
                    ex.Errors),

                NotFoundException ex => (
                    StatusCodes.Status404NotFound,
                    "Recurso no encontrado",
                    ex.Message,
                    null),

                UnauthorizedException ex => (
                    StatusCodes.Status401Unauthorized,
                    "No autorizado",
                    ex.Message,
                    null),

                ForbiddenException ex => (
                    StatusCodes.Status403Forbidden,
                    "Acceso denegado",
                    ex.Message,
                    null),

                ConflictException ex => (
                    StatusCodes.Status409Conflict,
                    "Conflicto",
                    ex.Message,
                    null),

                BusinessRuleException ex => (
                    StatusCodes.Status422UnprocessableEntity,
                    "Regla de negocio incumplida",
                    ex.Message,
                    null),

                _ => (
                    StatusCodes.Status500InternalServerError,
                    "Error interno del servidor",
                    "Se produjo un error inesperado.",
                    null)
            };

            if (status >= 500)
            {
                _logger.LogError(
                    exception,
                    "Error no controlado. TraceId: {TraceId}",
                    Activity.Current?.Id ?? context.TraceIdentifier);
            }
            else
            {
                _logger.LogWarning(
                    "Excepción controlada: {ExceptionType}. " +
                    "StatusCode: {StatusCode}. TraceId: {TraceId}",
                    exception.GetType().Name,
                    status,
                    Activity.Current?.Id ?? context.TraceIdentifier);
            }

            context.Response.StatusCode = status;

            var problem = new ProblemDetails
            {
                Status = status,
                Title = title,
                Detail = detail,
                Instance = context.Request.Path
            };

            problem.Extensions["traceId"] =
                Activity.Current?.Id ?? context.TraceIdentifier;

            if (errors is not null)
            {
                problem.Extensions["errors"] = errors;
            }

            await context.Response.WriteAsJsonAsync(
                problem,
                cancellationToken);

            return true;
        }
    }

}
