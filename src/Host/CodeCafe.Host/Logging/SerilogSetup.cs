using Serilog;

namespace CodeCafe.Host.Logging;

/// <summary>
/// Composition-root helpers for configuring Serilog from configuration.
/// Kept inside the Host project on purpose: logging wiring is a host concern,
/// not a module or adapter concern. Additional sinks (file, seq, otel) can be
/// added here later without touching modules or adapters.
/// </summary>
public static class SerilogSetup
{
    /// <summary>
    /// Build the Serilog logger from the "Serilog" section of configuration.
    /// Falls back to a sensible default when the section is missing so that
    /// the application always has a working logger at startup.
    /// </summary>
    public static void ConfigureSerilog(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((context, services, loggerConfiguration) =>
        {
            loggerConfiguration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext();
        });
    }
}
