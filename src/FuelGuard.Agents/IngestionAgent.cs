using FuelGuard.Shared.Abstractions;
using FuelGuard.Shared.Events;
using Microsoft.Extensions.Logging;

namespace FuelGuard.Agents;

public sealed class IngestionAgent(ILogger<IngestionAgent> logger) : IAgent
{
    public string Name => "Ingestion";

    public void Register(IEventBus eventBus) =>
        eventBus.Subscribe<FuelTransactionCreatedEvent>(OnTransactionCreatedAsync);

    private Task OnTransactionCreatedAsync(IServiceProvider _, FuelTransactionCreatedEvent evt, CancellationToken ct)
    {
        logger.LogInformation(
            "[INGESTION] Tx {TransactionId} | Company {CompanyId} | Tank {TankId} | {Quantity} L | {Movement} | Correlation {CorrelationId}",
            evt.TransactionId,
            evt.CompanyId,
            evt.TankId,
            evt.Quantity,
            evt.MovementType,
            evt.CorrelationId);

        return Task.CompletedTask;
    }
}
