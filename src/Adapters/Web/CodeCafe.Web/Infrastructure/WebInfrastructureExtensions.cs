namespace CodeCafe.Web.Infrastructure;

/// <summary>
/// Web adapter composition helpers. The Host calls these to install the
/// HTTP-specific cross-cutting concerns (exception handling) without having
/// to know the concrete types. Adapters stay free of business logic.
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
        return services;
    }

    /// <summary>
    /// Activate the exception handler middleware. Call this early in the
    /// pipeline so subsequent middleware can throw without leaking raw
    /// exception payloads to clients.
    /// </summary>
    public static WebApplication UseCodeCafeWebInfrastructure(this WebApplication app)
    {
        app.UseExceptionHandler();
        return app;
    }
}
