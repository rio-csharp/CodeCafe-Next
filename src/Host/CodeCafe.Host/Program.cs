using CodeCafe.BuildingBlocks.Mediator;
using CodeCafe.Host.Logging;
using CodeCafe.Modules.Platform.Infrastructure.DependencyInjection;
using CodeCafe.Modules.Platform.Infrastructure.Persistence;
using CodeCafe.Web.Controllers;
using CodeCafe.Web.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Logging: configuration-driven Serilog wired at the composition root.
builder.ConfigureSerilog();

builder.Services.AddCodeCafeWebInfrastructure();
builder.Services
    .AddControllers()
    .AddApplicationPart(typeof(AuthController).Assembly);
builder.Services.AddOpenApi();

// MediatR scans the supplied module application assemblies. The Platform
// application assembly is the one that ships the auth handlers.
builder.Services.AddCodeCafeMediatR(
    typeof(CodeCafe.Modules.AI.Application.AssemblyMarker).Assembly,
    typeof(CodeCafe.Modules.Code.Application.AssemblyMarker).Assembly,
    typeof(CodeCafe.Modules.Notes.Application.AssemblyMarker).Assembly,
    typeof(CodeCafe.Modules.Platform.Application.AssemblyMarker).Assembly);

// Platform module: persistence and supporting services. The connection
// string is mandatory. We do not want a misconfigured host to silently
// fall back to a stray local sqlite file.
var platformConnectionString = builder.Configuration.GetSection("Platform")["ConnectionString"];
if (string.IsNullOrWhiteSpace(platformConnectionString))
{
    throw new InvalidOperationException(
        "Missing required configuration value 'Platform:ConnectionString'. " +
        "Set it in appsettings.json, environment, or user secrets before starting the host.");
}

builder.Services.AddCodeCafePlatformInfrastructure(platformConnectionString);

var app = builder.Build();

// Apply migrations on startup. Migrations (not EnsureCreated) own the
// schema history, so a database created in an earlier run will be
// detected and brought forward correctly.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PlatformDbContext>();
    await db.Database.MigrateAsync();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCodeCafeWebInfrastructure();
app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
