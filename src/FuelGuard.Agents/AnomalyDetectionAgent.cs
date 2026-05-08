using FuelGuard.Application.Abstractions;
using FuelGuard.Domain.Entities;
using FuelGuard.Domain.Extensions;
using FuelGuard.Shared.Abstractions;
using FuelGuard.Shared.Events;
using FuelGuard.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FuelGuard.Agents;

public sealed class AnomalyDetectionAgent(ILogger<AnomalyDetectionAgent> logger) : IAgent
{
    public string Name => "AnomalyDetection";

    public void Register(IEventBus eventBus) =>
        eventBus.Subscribe<AggregationCompletedEvent>(OnAggregationCompletedAsync);

    private async Task OnAggregationCompletedAsync(IServiceProvider sp, AggregationCompletedEvent evt, CancellationToken ct)
    {
        var db = sp.GetRequiredService<IApplicationDbContext>();
        var bus = sp.GetRequiredService<IEventBus>();

        var windowStartDate = evt.SnapshotDate.AddDays(-14);
        var rangeStartUtc = DateTime.SpecifyKind(windowStartDate.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc);
        var rangeEndExclusiveUtc = DateTime.SpecifyKind(evt.SnapshotDate.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc)
            .AddDays(1);

        var txs = await db.FuelTransactions
            .Where(t => t.TankId == evt.TankId
                        && t.OccurredAt >= new DateTimeOffset(rangeStartUtc)
                        && t.OccurredAt < new DateTimeOffset(rangeEndExclusiveUtc))
            .AsNoTracking()
            .ToListAsync(ct);

        var netsByDay = txs
            .GroupBy(t => DateOnly.FromDateTime(t.OccurredAt.UtcDateTime))
            .ToDictionary(
                g => g.Key,
                g => g.Sum(x => x.MovementType.ToSignedLiters(x.QuantityLiters)));

        var todayNet = netsByDay.GetValueOrDefault(evt.SnapshotDate);
        var todayAbs = Math.Abs(todayNet);

        var history = netsByDay
            .Where(kvp => kvp.Key < evt.SnapshotDate && kvp.Key >= windowStartDate)
            .Select(kvp => Math.Abs(kvp.Value))
            .ToList();

        var avg = history.Count > 0 ? history.Average() : 0m;

        AnomalySeverity? severity = null;
        var explanation = string.Empty;

        if (history.Count < 2)
        {
            if (todayAbs > 12_000m)
            {
                severity = AnomalySeverity.Medium;
                explanation = "Sparse history baseline with a large single-day movement.";
            }
        }
        else if (avg > 0 && todayAbs > avg * 3.5m)
        {
            severity = todayAbs > avg * 6m ? AnomalySeverity.High : AnomalySeverity.Medium;
            explanation = $"Daily net {todayAbs:N0} L exceeds trailing average {avg:N0} L.";
        }
        else if (avg == 0 && todayAbs > 8_000m)
        {
            severity = AnomalySeverity.Low;
            explanation = "Elevated movement after a calm trailing window.";
        }

        if (severity is null)
        {
            logger.LogInformation(
                "[ANOMALY] No signal | Tank {TankId} | {Date} | TodayAbs {TodayAbs} | Avg {Avg} | Correlation {CorrelationId}",
                evt.TankId,
                evt.SnapshotDate,
                todayAbs,
                avg,
                evt.CorrelationId);

            return;
        }

        var anomaly = new AnomalyDetection
        {
            Id = Guid.NewGuid(),
            CompanyId = evt.CompanyId,
            TankId = evt.TankId,
            SnapshotDate = evt.SnapshotDate,
            ExpectedValue = avg,
            ActualValue = todayAbs,
            Severity = severity.Value,
            Explanation = explanation
        };

        db.AnomalyDetections.Add(anomaly);
        await db.SaveChangesAsync(ct);

        logger.LogWarning(
            "[ANOMALY] {Severity} | Tank {TankId} | {Date} | {Explanation} | Correlation {CorrelationId}",
            severity,
            evt.TankId,
            evt.SnapshotDate,
            explanation,
            evt.CorrelationId);

        await bus.PublishAsync(
            new AnomalyDetectedEvent(
                anomaly.Id,
                evt.CompanyId,
                evt.TankId,
                evt.SnapshotDate,
                avg,
                todayAbs,
                severity.Value,
                explanation)
            {
                CorrelationId = evt.CorrelationId
            },
            ct);
    }
}
