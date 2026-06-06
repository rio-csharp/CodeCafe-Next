using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CodeCafe.Modules.Platform.Infrastructure.Persistence.DesignTime;

/// <summary>
/// Design-time factory so <c>dotnet ef</c> can construct a
/// <see cref="PlatformDbContext"/> without booting the Host. Used only by
/// the EF Core CLI; production code goes through
/// <see cref="PlatformDbContext"/>'s runtime DI registration.
/// </summary>
public sealed class PlatformDbContextDesignTimeFactory : IDesignTimeDbContextFactory<PlatformDbContext>
{
    public PlatformDbContext CreateDbContext(string[] args)
    {
        // The CLI will not actually connect with this connection string;
        // it only needs the provider configured so that
        // `dotnet ef migrations add ...` can build the model. We point at
        // a throwaway local file to keep the CLI honest.
        var options = new DbContextOptionsBuilder<PlatformDbContext>()
            .UseSqlite("Data Source=design-time-codecafe.db")
            .Options;
        return new PlatformDbContext(options);
    }
}
