using FuelGuard.Agents;
using FuelGuard.Application;
using FuelGuard.Infrastructure;
using FuelGuard.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddSimpleConsole(options =>
{
    options.TimestampFormat = "HH:mm:ss ";
    options.SingleLine = true;
});

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddAgents();

builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "FuelGuardWeb",
        policy => policy
            .WithOrigins(
                "http://localhost:4200",
                "https://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod());
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "FuelGuard AI",
        Version = "v1",
        Description = "Multi-agent, event-driven fuel integrity API (hackathon build)."
    });
});

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    var db = scope.ServiceProvider.GetRequiredService<FuelGuardDbContext>();

    if (db.Database.IsRelational())
        await db.Database.MigrateAsync();
    else
        await db.Database.EnsureCreatedAsync();

    var seeder = scope.ServiceProvider.GetRequiredService<DemoDataSeeder>();
    await seeder.SeedAsync(db);
}

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "FuelGuard AI v1");
});

app.UseCors("FuelGuardWeb");

app.MapControllers();

app.MapGet("/health", () => Results.Ok(new { status = "ok", product = "FuelGuard AI" }));

app.Run();
