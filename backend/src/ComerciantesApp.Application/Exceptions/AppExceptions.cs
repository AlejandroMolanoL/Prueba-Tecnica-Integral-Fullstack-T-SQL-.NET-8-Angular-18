namespace ComerciantesApp.Application.Exceptions;

public class AppException : Exception
{
    public int StatusCode { get; }
    public List<string>? Errors { get; }

    public AppException(string message, int statusCode = 400, List<string>? errors = null) 
        : base(message)
    {
        StatusCode = statusCode;
        Errors = errors;
    }
}

public class NotFoundException : AppException
{
    public NotFoundException(string message) : base(message, 404) { }
}

public class UnauthorizedAppException : AppException
{
    public UnauthorizedAppException(string message = "No autorizado") : base(message, 401) { }
}

public class ForbiddenAppException : AppException
{
    public ForbiddenAppException(string message = "Acceso no permitido para este rol") : base(message, 403) { }
}

public class ValidationAppException : AppException
{
    public ValidationAppException(List<string> errors) 
        : base("Se presentaron uno o más errores de validación", 400, errors) { }

    public ValidationAppException(string error) 
        : base("Se presentó un error de validación", 400, new List<string> { error }) { }
}
