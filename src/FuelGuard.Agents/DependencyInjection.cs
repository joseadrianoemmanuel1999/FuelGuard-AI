using FuelGuard.Agents.Hosting;
using FuelGuard.Shared.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace FuelGuard.Agents;

public static class DependencyInjection
{
    public static IServiceCollection AddAgents(this IServiceCollection services)
    {
        services.AddSingleton<IAgent, IngestionAgent>();
        services.AddSingleton<IAgent, AggregationAgent>();
        services.AddSingleton<IAgent, AnomalyDetectionAgent>();
        services.AddSingleton<IAgent, RiskScoringAgent>();
        services.AddSingleton<IAgent, AlertAgent>();

        services.AddHostedService<AgentCoordinator>();

        return services;
    }
}
