using Domain.Entities;
using System.Net;
using System.Text.Json;
using Application.Exceptions;
namespace API.Middleware;

public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlerMiddleware> _logger;

    public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
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
            _logger.LogError(ex, "An unhandled exception has occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = context.Response;
        response.ContentType = "application/json";

        // По умолчанию статус 500
        var statusCode = HttpStatusCode.InternalServerError;
        string message = "An internal server error occurred. Please try again later.";

        switch (exception)
        {
            case NotFoundException:
                statusCode = HttpStatusCode.NotFound;
                message = exception.Message;
                break;
            case ValidationException:
                statusCode = HttpStatusCode.BadRequest;
                message = exception.Message;
                break;
            case DomainValidationException:
                statusCode = HttpStatusCode.BadRequest;
                message = exception.Message;
                break;
            case Application.Exceptions.UnauthorizedAccessException:
                statusCode = HttpStatusCode.Forbidden;
                message = exception.Message; 
                break;
        }

        response.StatusCode = (int)statusCode;
        
        var result = JsonSerializer.Serialize(new { Error = message });
        await response.WriteAsync(result);
    }
}