using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace LoanManagement.Api.Middleware;

public class RequestResponseLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestResponseLoggingMiddleware> _logger;
    
    private static readonly Dictionary<string, string[]> LoggedEndpoints = new()
    {
        { "POST", new[] { "/api/auth/login", "/api/auth/register", "/api/loans" } },
        { "PUT", new[] { "/api/loans" } }
    };

    private static readonly string[] SensitiveFields = { "password", "token" };

    public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var requestId = Guid.NewGuid().ToString("N")[..8];
        
        LogRequestMetadata(context, requestId);

        try
        {
            if (ShouldLogPath(context))
            {
                await LogRequestAsync(context, requestId);
            }

            await _next(context);
            
            LogResponseMetadata(context, requestId, stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[{RequestId}] Exception: {Message}", requestId, ex.Message);
        }
    }

    private static bool ShouldLogPath(HttpContext context)
    {
        var method = context.Request.Method.ToUpper();
        var path = context.Request.Path.Value?.ToLower() ?? "";

        if (!LoggedEndpoints.TryGetValue(method, out var endpoints)) return false;

        return endpoints.Any(loggedPath => path.StartsWith(loggedPath.ToLower(), StringComparison.OrdinalIgnoreCase));
    }

    private void LogRequestMetadata(HttpContext context, string requestId)
    {
        _logger.LogInformation("[{RequestId}] {Method} {Path} | User: {UserId}", 
            requestId, 
            context.Request.Method, 
            context.Request.Path,
            context.User?.FindFirst("id")?.Value ?? "Anonymous");
    }

    private void LogResponseMetadata(HttpContext context, string requestId, long durationMs)
    {
        _logger.LogInformation("[{RequestId}] Response: {StatusCode} in {Duration}ms",
            requestId, context.Response.StatusCode, durationMs);
    }

    private async Task LogRequestAsync(HttpContext context, string requestId)
    {
        var request = context.Request;
        
        if ((request.Method == "POST" || request.Method == "PUT") && request.ContentLength > 0)
        {
            var body = await ReadRequestBodyAsync(request);
            if (!string.IsNullOrEmpty(body))
            {
                var sanitized = SanitizeJson(body);
                _logger.LogInformation("[{RequestId}] Request Body: {Body}", requestId, sanitized);
            }
        }
        
        if (context.Request.Query.Any())
        {
            var queryParams = context.Request.Query.ToDictionary(q => q.Key, q => q.Value.ToString());
            _logger.LogInformation("[{RequestId}] Query Parameters: {QueryParams}", 
                requestId, JsonSerializer.Serialize(queryParams));
        }
    }

    private static string SanitizeJson(string json)
    {
        try
        {
            using var document = JsonDocument.Parse(json);
            var sanitized = SanitizeJsonElement(document.RootElement);
            return JsonSerializer.Serialize(sanitized, new JsonSerializerOptions { WriteIndented = false });
        }
        catch
        {
            return json;
        }
    }

    private static object SanitizeJsonElement(JsonElement element)
    {
        return element.ValueKind switch
        {
            JsonValueKind.Object => SanitizeObject(element),
            JsonValueKind.Array => element.EnumerateArray().Select(SanitizeJsonElement).ToArray(),
            _ => GetValue(element)
        };
    }

    private static Dictionary<string, object> SanitizeObject(JsonElement element)
    {
        var result = new Dictionary<string, object>();
        foreach (var property in element.EnumerateObject())
        {
            var key = property.Name;
            var value = SensitiveFields.Any(field => key.Contains(field, StringComparison.OrdinalIgnoreCase))
                ? "[REDACTED]"
                : SanitizeJsonElement(property.Value);
            result[key] = value;
        }
        return result;
    }

    private static object GetValue(JsonElement element)
    {
        return element.ValueKind switch
        {
            JsonValueKind.String => element.GetString(),
            JsonValueKind.Number => element.GetDecimal(),
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.Null => null,
            _ => element.ToString()
        };
    }

    private static async Task<string> ReadRequestBodyAsync(HttpRequest request)
    {
        request.EnableBuffering();
        request.Body.Position = 0;
        
        using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
        var body = await reader.ReadToEndAsync();
        request.Body.Position = 0;
        
        return body;
    }
}