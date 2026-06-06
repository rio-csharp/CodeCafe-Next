using CodeCafe.Modules.Platform.Application.Abstractions;
using CodeCafe.Web.Auth;

namespace CodeCafe.Web.Infrastructure;

/// <summary>
/// Web adapter composition helpers. The Host calls these to install the
/// HTTP-specific cross-cutting concerns (exception handling, auth) without
/// having to know the concrete types. Adapters stay free of business logic.
/// </summary>
public static class WebInfrastructureExtensions
{
    /// <summary>
    /// Register the unified exception handler used by the Web adapter and
    /// consumed by the Host pipeline. Call this before <c>Build()</c>.
    /// </summary>
    public static IServiceCollection AddCodeCafeWebInfrastructure(this IServiceCollection services)
    {
        services.AddExceptionHandler<GlobalExceptionHandler>();
        // UseExceptionHandler requires a fallback handler (path, inline
        // handler, or problem-details) even when we have an IExceptionHandler
        // registered. AddProblemDetails gives us a JSON-shaped default that
        // is overridden by our GlobalExceptionHandler for our own exception
        // types and acts as a safety net for anything that slips past.
        services.AddProblemDetails();
        services.AddHttpContextAccessor();
        services.AddScoped<CookieAuthTicketIssuer>();
        services.AddScoped<ICurrentUser, HttpContextCurrentUser>();
        services.AddCodeCafeWebAuth();
        return services;
    }

    /// <summary>
    /// Activate the exception handler middleware and the cookie auth
    /// pipeline. Call this early in the pipeline so subsequent middleware
    /// can throw without leaking raw exception payloads to clients.
    /// </summary>
    public static WebApplication UseCodeCafeWebInfrastructure(this WebApplication app)
    {
        app.UseExceptionHandler();
        app.UseCodeCafeWebAuth();
        return app;
    }
}
