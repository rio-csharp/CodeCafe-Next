using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace CodeCafe.BuildingBlocks.Mediator;

/// <summary>
/// MediatR registration helper shared by the composition root (Host) and any
/// module that needs to wire its own handlers. Centralising the registration
/// keeps the pipeline behavior ordering consistent across modules.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Register MediatR and scan the given assemblies for handlers, notifications,
    /// and pipeline behaviors. Pass module Application assemblies from the Host
    /// composition root.
    /// </summary>
    /// <remarks>
    /// MediatR requires at least one assembly to scan. When called with no
    /// assemblies we fall back to the BuildingBlocks assembly itself as a
    /// safe placeholder so the dispatcher is registered and the structure is
    /// ready for future feature work. As soon as the first module has handlers,
    /// pass that module's Application assembly here to register them.
    /// </remarks>
    public static IServiceCollection AddCodeCafeMediatR(
        this IServiceCollection services,
        params Assembly[] handlerAssemblies)
    {
        var assemblies = handlerAssemblies.Length > 0
            ? handlerAssemblies
            : new[] { typeof(ServiceCollectionExtensions).Assembly };

        services.AddMediatR(cfg =>
        {
            // Register handlers, notifications, and request pre/post processors
            // from each supplied module application assembly.
            cfg.RegisterServicesFromAssemblies(assemblies);

            // Reserved place for future cross-cutting pipeline behaviors.
            // Add behaviors here in the order they should run (outermost first):
            //   1. LoggingBehavior<,>      - request/response logging
            //   2. ValidationBehavior<,>   - input validation
            //   3. TransactionBehavior<,>  - unit-of-work wrapping
            // Example:
            //   cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
            //   cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            //   cfg.AddOpenBehavior(typeof(TransactionBehavior<,>));
        });

        return services;
    }
}
