using CodeCafe.Modules.Platform.Application.Abstractions;
using CodeCafe.Modules.Platform.Infrastructure.Auth;
using CodeCafe.Modules.Platform.Infrastructure.Persistence;
using CodeCafe.Modules.Platform.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CodeCafe.Modules.Platform.Infrastructure.DependencyInjection;

/// <summary>
/// Composition helper for the Platform module. Wires the DbContext, the
/// module-specific repository, and the password hasher. The HTTP-backed
/// current-user lives in the Web adapter, since it depends on ASP.NET Core
/// types that should not leak into a non-web infrastructure assembly.
/// </summary>
public static class PlatformInfrastructureExtensions
{
    /// <summary>
    /// Register Platform persistence and infrastructure services. The
    /// <paramref name="connectionString"/> is required: misconfiguration
    /// must surface at startup, not as silent writes to a stray local file.
    /// </summary>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="connectionString"/> is null, empty, or
    /// whitespace.
    /// </exception>
    public static IServiceCollection AddCodeCafePlatformInfrastructure(
        this IServiceCollection services,
        string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentException(
                "Platform connection string is required. " +
                "Configure 'Platform:ConnectionString' for the host.",
                nameof(connectionString));
        }

        services.AddDbContext<PlatformDbContext>(options => options.UseSqlite(connectionString));

        services.TryAddSingleton<IClock, SystemClock>();
        services.TryAddSingleton<IPasswordHasher, Pbkdf2PasswordHasher>();
        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }
}
