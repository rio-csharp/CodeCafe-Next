using CodeCafe.Modules.Platform.Domain.Entities;
using CodeCafe.Modules.Platform.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace CodeCafe.Modules.Platform.Infrastructure.Persistence;

/// <summary>
/// Module-scoped DbContext. Owns only the Platform tables so other modules
/// are not pulled into the same persistence model. The Web adapter does not
/// reference this type: it only ever sees the application-layer interfaces.
/// </summary>
public sealed class PlatformDbContext : DbContext
{
    public PlatformDbContext(DbContextOptions<PlatformDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // SQLite does not honour schemas, so we keep tables in the default
        // schema. The Platform module's identity tables are namespaced via
        // DbContext (own schema) once we move to a database engine that
        // supports them.
        modelBuilder.ApplyConfiguration(new UserConfiguration());
    }
}
