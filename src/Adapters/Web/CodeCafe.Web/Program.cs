using CodeCafe.Web.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCodeCafeWebInfrastructure();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCodeCafeWebInfrastructure();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
