using System.Net;
using System.Text.Json;
using ComerciantesApp.Application.Common;
using ComerciantesApp.Application.Exceptions;

namespace ComerciantesApp.WebApi.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Excepción no controlada: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = new ApiResponse<object>();

        switch (exception)
        {
            case ValidationAppException valEx:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response = ApiResponse<object>.ErrorResult(valEx.Message, valEx.Errors);
                break;

            case NotFoundException notFoundEx:
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                response = ApiResponse<object>.ErrorResult(notFoundEx.Message);
                break;

            case UnauthorizedAppException unauthEx:
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                response = ApiResponse<object>.ErrorResult(unauthEx.Message);
                break;

            case ForbiddenAppException forbiddenEx:
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                response = ApiResponse<object>.ErrorResult(forbiddenEx.Message);
                break;

            case AppException appEx:
                context.Response.StatusCode = appEx.StatusCode;
                response = ApiResponse<object>.ErrorResult(appEx.Message, appEx.Errors);
                break;

            default:
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response = ApiResponse<object>.ErrorResult("Ocurrió un error inesperado en el servidor.");
                break;
        }

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        return context.Response.WriteAsync(json);
    }
}

