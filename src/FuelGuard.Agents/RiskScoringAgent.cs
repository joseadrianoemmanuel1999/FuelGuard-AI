using FuelGuard.Application.Abstractions;
using FuelGuard.Domain.Entities;
using FuelGuard.Shared.Abstractions;
using FuelGuard.Shared.Events;
using FuelGuard.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FuelGuard.Agents;

public sealed class RiskScoringAgent(ILogger<RiskScoringAgent> logger, IAiService aiService) : IAgent
{
    public string Name => "RiskScoring";

    public void Register(IEventBus eventBus) =>
        eventBus.Subscribe<AnomalyDetectedEvent>(OnAnomalyAsync);

    private async Task OnAnomalyAsync(IServiceProvider sp, AnomalyDetectedEvent evt, CancellationToken ct)
    {
        var db = sp.GetRequiredService<IApplicationDbContext>();
        var bus = sp.GetRequiredService<IEventBus>();

        var risk = await db.RiskScores.SingleOrDefaultAsync(r => r.CompanyId == evt.CompanyId, ct);
        if (risk is null)
        {
            risk = new RiskScore
            {
                CompanyId = evt.CompanyId,
                Score = 0,
                Level = RiskLevel.Low,
                Reason = string.Empty
            };

            db.RiskScores.Add(risk);
        }

        var previousScore = risk.Score;

        var delta = evt.Severity switch
        {
            AnomalySeverity.Low => 6m,
            AnomalySeverity.Medium => 16m,
            AnomalySeverity.High => 32m,
            _ => 10m
        };

        risk.Score = Math.Clamp(risk.Score + delta, 0, 100);
        risk.Level = MapLevel(risk.Score);

        var aiNarrative = await aiService.ExplainRiskUpdateAsync(
            new RiskAssessmentContext(
                evt.CompanyId,
                evt.AnomalyId,
                evt.Severity.ToString(),
                evt.Explanation,
                previousScore,
                risk.Score),
            ct);

        risk.Reason = aiNarrative
                      ?? $"Risk raised to {risk.Score:0} ({risk.Level}) after {evt.Severity} anomaly: {evt.Explanation}";

        await db.SaveChangesAsync(ct);

        logger.LogInformation(
            "[RISK] Company {CompanyId} | Score {Score} | Level {Level} | +{Delta} | Correlation {CorrelationId}",
            evt.CompanyId,
            risk.Score,
            risk.Level,
            delta,
            evt.CorrelationId);

        await bus.PublishAsync(
            new RiskCalculatedEvent(evt.CompanyId, risk.Score, risk.Level, evt.AnomalyId, risk.Reason)
            {
                CorrelationId = evt.CorrelationId
            },
            ct);
    }

    private static RiskLevel MapLevel(decimal score) =>
        score switch
        {
            >= 75 => RiskLevel.Critical,
            >= 50 => RiskLevel.High,
            >= 25 => RiskLevel.Medium,
            _ => RiskLevel.Low
        };
}
