using FuelGuard.Application.Abstractions;
using FuelGuard.Domain.Entities;
using FuelGuard.Application.Abstractions.Gemini;
using FuelGuard.Infrastructure.Ai;
using FuelGuard.Infrastructure.Ai.Gemini;
using FuelGuard.Infrastructure.Events;
using FuelGuard.Infrastructure.Persistence;
using FuelGuard.Shared.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

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

        services.Configure<AiOptions>(configuration.GetSection(AiOptions.SectionName));
        services.AddHttpClient(HuggingFaceAiService.HttpClientName, (sp, client) =>
        {
            var opts = sp.GetRequiredService<IOptions<AiOptions>>().Value;
            var seconds = Math.Clamp(opts.RequestTimeoutSeconds, 15, 180);
            client.Timeout = TimeSpan.FromSeconds(seconds);
        });

        services.AddSingleton<HuggingFaceAiService>();
        services.AddSingleton<IAiService>(sp =>
        {
            var opts = sp.GetRequiredService<IOptions<AiOptions>>().Value;
            if (string.IsNullOrWhiteSpace(opts.ApiKey))
                return new NoOpAiService();

            return sp.GetRequiredService<HuggingFaceAiService>();
        });

        services.Configure<GeminiOptions>(configuration.GetSection(GeminiOptions.SectionName));
        services.AddHttpClient(GeminiClient.HttpClientName, (sp, client) =>
        {
            var opts = sp.GetRequiredService<IOptions<GeminiOptions>>().Value;
            var seconds = Math.Clamp(opts.RequestTimeoutSeconds, 15, 120);
            client.Timeout = TimeSpan.FromSeconds(seconds);
        });
        services.AddScoped<GeminiOperationalContextBuilder>();
        services.AddSingleton<GeminiClient>();
        services.AddScoped<GeminiChatService>();
        services.AddScoped<GeminiInsightsService>();
        services.AddScoped<IGeminiService, GeminiService>();

        return services;
    }
}
