using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace CodeCafe.IntegrationTests;

/// <summary>
/// Test host for the Platform auth flow. Each test gets a fresh, isolated
/// SQLite file so register/login/me runs do not bleed across cases.
///
/// We use <see cref="CodeCafe.Host.HostEntryPointMarker"/> as the entry
/// point type instead of the implicit <c>Program</c> class, which collides
/// with the same implicit class in <c>CodeCafe.Mcp</c> (and any other
/// top-level-statement entry project in the solution).
/// </summary>
public sealed class PlatformAuthWebApplicationFactory : WebApplicationFactory<CodeCafe.Host.HostEntryPointMarker>
{
    private readonly string _databasePath = Path.Combine(
        Path.GetTempPath(),
        $"codecafe-it-{Guid.NewGuid():N}.db");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureAppConfiguration((_, config) =>
        {
            // Override the production connection string with a per-test
            // SQLite file. MigrateAsync runs at startup, so the schema is
            // always present.
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Platform:ConnectionString"] = $"Data Source={_databasePath}"
            });
        });
    }

    /// <summary>
    /// Clean up the per-instance sqlite file so test runs do not pile up
    /// artefacts under the user's temp folder.
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
        {
            try { File.Delete(_databasePath); } catch { /* best-effort */ }
        }
    }
}
