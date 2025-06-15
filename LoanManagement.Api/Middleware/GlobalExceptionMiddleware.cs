using System.Net;
using System.Text.Json;
using FluentValidation;

namespace LoanManagement.Api.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
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
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        
        var response = exception switch
        {
            ValidationException validationEx => CreateValidationErrorResponse(validationEx),
            UnauthorizedAccessException => CreateErrorResponse(HttpStatusCode.Unauthorized, exception.Message),
            InvalidOperationException => CreateErrorResponse(HttpStatusCode.BadRequest, exception.Message),
            ArgumentException => CreateErrorResponse(HttpStatusCode.BadRequest, exception.Message),
            _ => CreateErrorResponse(HttpStatusCode.InternalServerError, "An error occurred while processing your request.")
        };

        context.Response.StatusCode = response.StatusCode;
        await context.Response.WriteAsync(JsonSerializer.Serialize(response.Body, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        }));
    }

    private static (int StatusCode, object Body) CreateValidationErrorResponse(ValidationException validationException)
    {
        var errors = validationException.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                group => group.Key,
                group => group.Select(e => e.ErrorMessage).ToArray()
            );

        var response = new
        {
            type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            title = "Validation Error",
            status = 400,
            detail = "One or more validation failures have occurred.",
            errors = errors
        };

        return (400, response);
    } 

    private static (int StatusCode, object Body) CreateErrorResponse(HttpStatusCode statusCode, string message)
    {
        var response = new
        {
            error = message,
            statusCode = (int)statusCode
        };

        return ((int)statusCode, response);
    }
}