using FuelGuard.Shared.Abstractions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FuelGuard.Agents.Hosting;

public sealed class AgentCoordinator(IEnumerable<IAgent> agents, IEventBus eventBus, ILogger<AgentCoordinator> logger)
    : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        foreach (var agent in agents)
        {
            logger.LogInformation("FuelGuard | Registering agent {AgentName}", agent.Name);
            agent.Register(eventBus);
        }

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
