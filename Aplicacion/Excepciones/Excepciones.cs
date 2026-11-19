namespace Aplicacion.Excepciones
{    

    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message) { }
    }

    public class ValidationException : Exception
    {
        public IDictionary<string, string[]> Errors { get; }

        public ValidationException(string message)
            : base(message)
        {
            Errors = new Dictionary<string, string[]>();
        }

        public ValidationException(
            IDictionary<string, string[]> errors)
            : base("Se produjeron errores de validación.")
        {
            Errors = errors;
        }
    }

    public class ConflictException : Exception
    {
        public ConflictException(string message) : base(message) { }
    }

    public class UnauthorizedException : Exception
    {
        public UnauthorizedException(string message = "No autorizado.")
            : base(message) { }
    }

    public class ForbiddenException : Exception
    {
        public ForbiddenException(string message = "Acceso denegado.")
            : base(message) { }
    }

    public class BusinessRuleException : Exception
    {
        public BusinessRuleException(string message) : base(message) { }
    }

}
