using System.Net;
using System.Text;
using System.Text.Encodings.Web;

namespace Plumsail.Interview.Web.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    private static readonly JavaScriptEncoder JsonEncoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        // Use Exception.ToString() to get full exception details including stack trace
        // This is more reliable than accessing StackTrace property directly with trimming
        var fullExceptionDetails = exception.ToString();
        
        // Manually build JSON to avoid trimming issues - escape strings manually
        var jsonBuilder = new StringBuilder();
        jsonBuilder.Append("{");
        jsonBuilder.Append($"\"message\":\"{EscapeJsonString(exception.Message ?? string.Empty)}\"");
        jsonBuilder.Append($",\"error\":\"{EscapeJsonString(exception.GetType().FullName ?? exception.GetType().Name)}\"");
        
        // Always include full exception details (includes stack trace)
        jsonBuilder.Append($",\"stackTrace\":\"{EscapeJsonString(fullExceptionDetails)}\"");
        
        // Also include StackTrace property if available (for structured access)
        var stackTrace = exception.StackTrace;
        if (!string.IsNullOrEmpty(stackTrace))
        {
            jsonBuilder.Append($",\"stackTraceOnly\":\"{EscapeJsonString(stackTrace)}\"");
        }
        
        // Include inner exception details if present
        if (exception.InnerException != null)
        {
            var innerExceptionDetails = exception.InnerException.ToString();
            jsonBuilder.Append($",\"innerException\":{{");
            jsonBuilder.Append($"\"type\":\"{EscapeJsonString(exception.InnerException.GetType().FullName ?? exception.InnerException.GetType().Name)}\"");
            jsonBuilder.Append($",\"message\":\"{EscapeJsonString(exception.InnerException.Message ?? string.Empty)}\"");
            jsonBuilder.Append($",\"stackTrace\":\"{EscapeJsonString(innerExceptionDetails)}\"");
            jsonBuilder.Append("}");
        }
        
        jsonBuilder.Append("}");

        return context.Response.WriteAsync(jsonBuilder.ToString());
    }

    private static string EscapeJsonString(string value)
    {
        if (string.IsNullOrEmpty(value))
            return string.Empty;

        var encoded = JsonEncoder.Encode(value);
        return encoded;
    }
}

