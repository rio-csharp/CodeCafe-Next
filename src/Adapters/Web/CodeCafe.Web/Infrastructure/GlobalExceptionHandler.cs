using Microsoft.AspNetCore.Diagnostics;

namespace CodeCafe.Web.Infrastructure;

/// <summary>
/// Converts unhandled exceptions into the unified <see cref="ErrorResponse"/> shape.
/// Registered as a singleton <see cref="IExceptionHandler"/> so it is invoked by
/// <c>UseExceptionHandler</c> in the Web and Host pipelines.
/// Adapters stay free of business logic: this handler only knows how to map an
/// exception to an HTTP transport shape, nothing about domain rules.
/// </summary>
public sealed partial class GlobalExceptionHandler : IExceptionHandler
{
    private readonly IHostEnvironment _environment;
    private readonly ILogger<GlobalExceptionHandler> _logger;

    /// <summary>
    /// Construct a new handler. Both <paramref name="environment"/> and
    /// <paramref name="logger"/> are resolved from the DI container; the
    /// handler itself stays free of static state.
    /// </summary>
    public GlobalExceptionHandler(IHostEnvironment environment, ILogger<GlobalExceptionHandler> logger)
    {
        _environment = environment;
        _logger = logger;
    }

    /// <summary>
    /// Map the exception to a stable <see cref="ErrorResponse"/> payload, log the
    /// full exception, and write a 500 response. Always returns <c>true</c> so the
    /// exception is considered handled and the pipeline does not rethrow.
    /// </summary>
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        // Always log with full exception. Production logs may be filtered to sinks
        // that never reach the client.
        LogUnhandledException(
            _logger,
            exception,
            httpContext.Request.Method,
            httpContext.Request.Path);

        var traceId = httpContext.TraceIdentifier;

        // Keep the public payload minimal in production. Development gets a
        // single "details" string with type and message so debugging is easy
        // without needing to open a full ProblemDetails payload.
        var response = new ErrorResponse(
            Code: "internal_error",
            Message: "An unexpected error occurred.",
            TraceId: traceId)
        {
            Details = _environment.IsDevelopment()
                ? $"{exception.GetType().FullName}: {exception.Message}"
                : null
        };

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        httpContext.Response.ContentType = "application/json";
        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

        return true;
    }

    [LoggerMessage(
        EventId = 1000,
        Level = LogLevel.Error,
        Message = "Unhandled exception while processing request {Method} {Path}")]
    private static partial void LogUnhandledException(
        ILogger logger,
        Exception exception,
        string method,
        string path);
}
