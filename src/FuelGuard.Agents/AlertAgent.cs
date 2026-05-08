using FuelGuard.Shared.Abstractions;
using FuelGuard.Shared.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FuelGuard.Agents;

public sealed class AlertAgent(ILogger<AlertAgent> logger) : IAgent
{
    public string Name => "Alert";

    public void Register(IEventBus eventBus) =>
        eventBus.Subscribe<RiskCalculatedEvent>(OnRiskCalculatedAsync);

    private async Task OnRiskCalculatedAsync(IServiceProvider sp, RiskCalculatedEvent evt, CancellationToken ct)
    {
        var bus = sp.GetRequiredService<IEventBus>();

        var message =
            $"Company {evt.CompanyId} risk is now {evt.Score:0} ({evt.Level}). {evt.Reason}";

        logger.LogWarning(
            "[ALERT] {Message} | Anomaly {AnomalyId} | Correlation {CorrelationId}",
            message,
            evt.TriggeringAnomalyId,
            evt.CorrelationId);

        await bus.PublishAsync(
            new AlertGeneratedEvent(evt.CompanyId, evt.Score, evt.Level, message, evt.TriggeringAnomalyId)
            {
                CorrelationId = evt.CorrelationId
            },
            ct);
    }
}
