using System.Text;
using FuelGuard.Application.Abstractions;
using FuelGuard.Application.Abstractions.Gemini;
using FuelGuard.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace FuelGuard.Infrastructure.Ai.Gemini;

/// <summary>
/// Loads authoritative operational facts from EF Core for Gemini grounding.
/// </summary>
public sealed class GeminiOperationalContextBuilder(IApplicationDbContext db)
{
    public async Task<OperationalFactsSnapshot> BuildAsync(
        string? stationHint,
        string? periodHint,
        CancellationToken cancellationToken)
    {
        var since = ResolvePeriodStart(periodHint);

        var companies = await db.Companies
            .AsNoTracking()
            .Select(c => new { c.Id, c.Name, c.Type })
            .ToListAsync(cancellationToken);

        var riskRows = await db.RiskScores
            .AsNoTracking()
            .OrderByDescending(r => r.Score)
            .Select(r => new
            {
                r.CompanyId,
                r.Score,
                r.Level,
                r.Reason
            })
            .ToListAsync(cancellationToken);

        var anomaliesQuery = db.AnomalyDetections.AsNoTracking();
        if (since.HasValue)
            anomaliesQuery = anomaliesQuery.Where(a => a.DetectedAt >= since.Value);

        var anomalies = await anomaliesQuery
            .OrderByDescending(a => a.DetectedAt)
            .Take(12)
            .Select(a => new
            {
                a.Id,
                a.CompanyId,
                a.TankId,
                a.Severity,
                a.ExpectedValue,
                a.ActualValue,
                a.Explanation,
                a.DetectedAt
            })
            .ToListAsync(cancellationToken);

        var txQuery = db.FuelTransactions.AsNoTracking();
        if (since.HasValue)
            txQuery = txQuery.Where(t => t.OccurredAt >= since.Value);

        var transactions = await txQuery
            .OrderByDescending(t => t.OccurredAt)
            .Take(20)
            .Select(t => new
            {
                t.CompanyId,
                t.TankId,
                t.QuantityLiters,
                t.MovementType,
                t.OccurredAt
            })
            .ToListAsync(cancellationToken);

        if (!string.IsNullOrWhiteSpace(stationHint))
        {
            var hint = stationHint.Trim();
            var companyIds = companies
                .Where(c => c.Name.Contains(hint, StringComparison.OrdinalIgnoreCase)
                            || c.Id.ToString().StartsWith(hint, StringComparison.OrdinalIgnoreCase))
                .Select(c => c.Id)
                .ToHashSet();

            if (companyIds.Count > 0)
            {
                anomalies = anomalies.Where(a => companyIds.Contains(a.CompanyId)).ToList();
                transactions = transactions.Where(t => companyIds.Contains(t.CompanyId)).ToList();
                riskRows = riskRows.Where(r => companyIds.Contains(r.CompanyId)).ToList();
            }
        }

        var dominantLevel = riskRows.Count > 0
            ? MapLevel(riskRows[0].Level)
            : "LOW";

        var facts = new StringBuilder();
        facts.AppendLine("OPERATIONAL FACTS (authoritative — do not invent data beyond these facts):");
        facts.AppendLine($"generatedAtUtc: {DateTimeOffset.UtcNow:O}");
        if (since.HasValue)
            facts.AppendLine($"periodFilterSinceUtc: {since:O}");
        if (!string.IsNullOrWhiteSpace(stationHint))
            facts.AppendLine($"stationFilterHint: {stationHint.Trim()}");

        facts.AppendLine();
        facts.AppendLine("COMPANIES / STATIONS:");
        foreach (var c in companies)
            facts.AppendLine($"- id={c.Id} name=\"{c.Name}\" type={c.Type}");

        facts.AppendLine();
        facts.AppendLine("RISK SCORES (deterministic — do not change scores):");
        foreach (var r in riskRows)
        {
            var name = companies.FirstOrDefault(c => c.Id == r.CompanyId)?.Name ?? r.CompanyId.ToString();
            facts.AppendLine(
                $"- company=\"{name}\" score={r.Score:0.##} level={MapLevel(r.Level)} reason=\"{r.Reason}\"");
        }

        facts.AppendLine();
        facts.AppendLine($"ANOMALY DETECTIONS (count={anomalies.Count}):");
        foreach (var a in anomalies)
        {
            var delta = Math.Abs(a.ActualValue - a.ExpectedValue);
            var name = companies.FirstOrDefault(c => c.Id == a.CompanyId)?.Name ?? a.CompanyId.ToString();
            facts.AppendLine(
                $"- id={a.Id} station=\"{name}\" severity={a.Severity} delta={delta:F2} " +
                $"expected={a.ExpectedValue} actual={a.ActualValue} at={a.DetectedAt:O} explanation=\"{a.Explanation}\"");
        }

        facts.AppendLine();
        facts.AppendLine($"FUEL TRANSACTIONS (count={transactions.Count}):");
        foreach (var t in transactions)
        {
            var name = companies.FirstOrDefault(c => c.Id == t.CompanyId)?.Name ?? t.CompanyId.ToString();
            facts.AppendLine(
                $"- station=\"{name}\" tank={t.TankId} liters={t.QuantityLiters} movement={t.MovementType} at={t.OccurredAt:O}");
        }

        if (anomalies.Count == 0 && transactions.Count == 0 && riskRows.Count == 0)
            facts.AppendLine("(no operational rows matched filters — state may be nominal or database empty)");

        return new OperationalFactsSnapshot(
            facts.ToString(),
            dominantLevel,
            anomalies.Count,
            transactions.Count);
    }

    private static DateTimeOffset? ResolvePeriodStart(string? periodHint)
    {
        if (string.IsNullOrWhiteSpace(periodHint))
            return DateTimeOffset.UtcNow.AddDays(-30);

        var hint = periodHint.Trim().ToLowerInvariant();
        if (hint is "all" or "full" or "lifetime")
            return null;

        if (hint.EndsWith('d') && int.TryParse(hint[..^1], out var days))
            return DateTimeOffset.UtcNow.AddDays(-days);

        if (hint.EndsWith('h') && int.TryParse(hint[..^1], out var hours))
            return DateTimeOffset.UtcNow.AddHours(-hours);

        return DateTimeOffset.UtcNow.AddDays(-30);
    }

    private static string MapLevel(RiskLevel level) => level switch
    {
        RiskLevel.Low => "LOW",
        RiskLevel.Medium => "MEDIUM",
        RiskLevel.High => "HIGH",
        RiskLevel.Critical => "CRITICAL",
        _ => "UNKNOWN"
    };
}
