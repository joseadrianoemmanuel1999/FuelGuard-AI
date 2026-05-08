using FuelGuard.Application.Abstractions;
using FuelGuard.Domain.Entities;
using FuelGuard.Infrastructure.Ai;
using FuelGuard.Infrastructure.Events;
using FuelGuard.Infrastructure.Persistence;
using FuelGuard.Shared.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FuelGuard.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var rawConnectionString = configuration.GetConnectionString("FuelGuard");

        services.AddDbContext<FuelGuardDbContext>((sp, options) =>
        {
            if (string.IsNullOrWhiteSpace(rawConnectionString))
            {
                var log = sp.GetService<ILogger<FuelGuardDbContext>>();
                log?.LogWarning(
                    "ConnectionStrings:FuelGuard is empty — using in-memory database. " +
                    "Set a Supabase PostgreSQL connection string for persistent storage.");

                options.UseInMemoryDatabase("FuelGuardLocal");
                return;
            }

            var connectionString = SupabaseConnectionString.ForEntityFramework(rawConnectionString);

            options.UseNpgsql(connectionString, npgsql =>
            {
                npgsql.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                npgsql.CommandTimeout(60);
            });
        });

        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<FuelGuardDbContext>());

        services.AddSingleton<IEventBus, InMemoryEventBus>();

        services.AddScoped<IRepository<Company, Guid>>(sp =>
            new EfRepository<Company>(sp.GetRequiredService<FuelGuardDbContext>()));

        services.AddSingleton<DemoDataSeeder>();

        services.AddSingleton<IAiService, NoOpAiService>();

        return services;
    }
}
