namespace backend.ExceptionHandlers;

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {

        // structured logging - method and path become searchable in log aggregators
        logger.LogError(exception, "Unhandled exception for {Method} {Path}",
            httpContext.Request.Method, httpContext.Request.Path);

        // map exceptions to status codes
        var statusCode = exception switch
        {
            KeyNotFoundException => StatusCodes.Status404NotFound,
            ArgumentException => StatusCodes.Status400BadRequest,
            InvalidOperationException => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError
        };

        // hide details for 500 — can leak stack traces or sensitive data. 
        var detail = statusCode == StatusCodes.Status500InternalServerError
            ? "An unexpected error occurred. Please try again later."
            : exception.Message;

        httpContext.Response.StatusCode = statusCode;

        // ProblemDetails is a standard format for error responses
        // compliant with RFC 7807, supported by many clients out of the box
        await httpContext.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = statusCode,
            // map status code to reason phrase, e.g. 404 -> Not Found
            Title = ReasonPhrases.GetReasonPhrase(statusCode),
            Detail = detail,
            Instance = httpContext.Request.Path
        }, cancellationToken);

        return true;
    }
}
