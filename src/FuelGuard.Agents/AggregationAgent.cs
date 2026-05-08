using FuelGuard.Application.Abstractions;
using FuelGuard.Domain.Entities;
using FuelGuard.Domain.Extensions;
using FuelGuard.Shared.Abstractions;
using FuelGuard.Shared.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FuelGuard.Agents;

public sealed class AggregationAgent(ILogger<AggregationAgent> logger) : IAgent
{
    public string Name => "Aggregation";

    public void Register(IEventBus eventBus) =>
        eventBus.Subscribe<FuelTransactionCreatedEvent>(OnTransactionCreatedAsync);

    private async Task OnTransactionCreatedAsync(IServiceProvider sp, FuelTransactionCreatedEvent evt, CancellationToken ct)
    {
        var db = sp.GetRequiredService<IApplicationDbContext>();
        var bus = sp.GetRequiredService<IEventBus>();

        var snapshotDate = DateOnly.FromDateTime(evt.TransactionDate.UtcDateTime);
        var dayStartUtc = DateTime.SpecifyKind(snapshotDate.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc);
        var dayEndUtc = dayStartUtc.AddDays(1);

        var windowStart = new DateTimeOffset(dayStartUtc);
        var windowEnd = new DateTimeOffset(dayEndUtc);

        var dayTransactions = await db.FuelTransactions
            .Where(t => t.TankId == evt.TankId && t.OccurredAt >= windowStart && t.OccurredAt < windowEnd)
            .AsNoTracking()
            .ToListAsync(ct);

        var netMovement = dayTransactions.Sum(t => t.MovementType.ToSignedLiters(t.QuantityLiters));

        var previousDate = snapshotDate.AddDays(-1);
        var previous = await db.DailyFuelSnapshots
            .AsNoTracking()
            .SingleOrDefaultAsync(s => s.TankId == evt.TankId && s.Date == previousDate, ct);

        var opening = previous?.ClosingStock ?? 0m;
        var closing = opening + netMovement;

        var snapshot = await db.DailyFuelSnapshots
            .SingleOrDefaultAsync(s => s.TankId == evt.TankId && s.Date == snapshotDate, ct);

        if (snapshot is null)
        {
            snapshot = new DailyFuelSnapshot
            {
                TankId = evt.TankId,
                Date = snapshotDate
            };

            db.DailyFuelSnapshots.Add(snapshot);
        }

        snapshot.OpeningStock = opening;
        snapshot.ClosingStock = closing;

        await db.SaveChangesAsync(ct);

        logger.LogInformation(
            "[AGGREGATION] Tank {TankId} | {Date} | Open {Open} L | Close {Close} L | Net {Net} L | Correlation {CorrelationId}",
            evt.TankId,
            snapshotDate,
            opening,
            closing,
            netMovement,
            evt.CorrelationId);

        var completed = new AggregationCompletedEvent(
            evt.CompanyId,
            evt.TankId,
            snapshotDate,
            opening,
            closing,
            netMovement)
        {
            CorrelationId = evt.CorrelationId
        };

        await bus.PublishAsync(completed, ct);
    }
}
