using CodeCafe.Modules.Platform.Application.Exceptions;
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
        // Known application-layer exceptions get a mapped status code and a
        // generic message. Everything else falls through to the 500 path so we
        // never leak the wrong thing to the client.
        var (status, code, message) = MapKnownException(exception);

        // Logging policy: 4xx-class outcomes are still recorded at warning so
        // we can spot patterns (credential stuffing, broken clients) without
        // flooding the error sink. True 5xx paths log the full exception.
        if (status >= 500)
        {
            LogUnhandledException(
                _logger,
                exception,
                httpContext.Request.Method,
                httpContext.Request.Path);
        }
        else
        {
            LogHandledClientError(
                _logger,
                exception,
                status,
                code,
                httpContext.Request.Method,
                httpContext.Request.Path);
        }

        var traceId = httpContext.TraceIdentifier;

        var response = new ErrorResponse(code, message, traceId)
        {
            Details = _environment.IsDevelopment()
                ? $"{exception.GetType().FullName}: {exception.Message}"
                : null
        };

        httpContext.Response.StatusCode = status;
        httpContext.Response.ContentType = "application/json";
        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

        return true;
    }

    private static (int Status, string Code, string Message) MapKnownException(Exception exception) =>
        exception switch
        {
            InvalidCredentialsException => (
                StatusCodes.Status401Unauthorized,
                "invalid_credentials",
                "Invalid email or password."),
            EmailAlreadyExistsException => (
                StatusCodes.Status409Conflict,
                "email_already_exists",
                "An account with that email already exists."),
            ArgumentException => (
                StatusCodes.Status400BadRequest,
                "invalid_request",
                "The request is invalid."),
            _ => (
                StatusCodes.Status500InternalServerError,
                "internal_error",
                "An unexpected error occurred.")
        };

    [LoggerMessage(
        EventId = 1000,
        Level = LogLevel.Error,
        Message = "Unhandled exception while processing request {Method} {Path}")]
    private static partial void LogUnhandledException(
        ILogger logger,
        Exception exception,
        string method,
        string path);

    [LoggerMessage(
        EventId = 1001,
        Level = LogLevel.Warning,
        Message = "Handled client error {Status} {Code} while processing {Method} {Path}")]
    private static partial void LogHandledClientError(
        ILogger logger,
        Exception exception,
        int status,
        string code,
        string method,
        string path);
}

