using CodeCafe.BuildingBlocks.Mediator;
using CodeCafe.Host.Logging;
using CodeCafe.Web.Infrastructure;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Logging: configuration-driven Serilog wired at the composition root.
builder.ConfigureSerilog();

builder.Services.AddCodeCafeWebInfrastructure();
builder.Services.AddControllers();
// MediatR is registered centrally at the composition root and scans the
// module Application assemblies, even if some modules do not have handlers yet.
builder.Services.AddCodeCafeMediatR(
    typeof(CodeCafe.Modules.AI.Application.AssemblyMarker).Assembly,
    typeof(CodeCafe.Modules.Code.Application.AssemblyMarker).Assembly,
    typeof(CodeCafe.Modules.Notes.Application.AssemblyMarker).Assembly,
    typeof(CodeCafe.Modules.Platform.Application.AssemblyMarker).Assembly);
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCodeCafeWebInfrastructure();
app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
