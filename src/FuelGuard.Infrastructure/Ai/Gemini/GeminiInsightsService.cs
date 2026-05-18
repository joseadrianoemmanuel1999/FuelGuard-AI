using FuelGuard.Application.Abstractions.Gemini;
using Microsoft.Extensions.Logging;

namespace FuelGuard.Infrastructure.Ai.Gemini;

public sealed class GeminiInsightsService(
    GeminiOperationalContextBuilder contextBuilder,
    GeminiClient client,
    ILogger<GeminiInsightsService> logger)
{
    public async Task<GeminiInsightsResponse> GetInsightsAsync(CancellationToken cancellationToken)
    {
        var snapshot = await contextBuilder.BuildAsync(
            stationHint: null,
            periodHint: "30d",
            cancellationToken);

        if (!client.IsConfigured)
            return BuildFallbackInsights(snapshot, poweredByGemini: false);

        var prompt = GeminiPromptBuilder.BuildInsightsPrompt(snapshot.FactsBlock);
        var raw = await client.GenerateJsonAsync(prompt, cancellationToken);
        var parsed = GeminiJsonParser.ParseInsights(raw);

        if (parsed is null)
        {
            logger.LogWarning("Gemini insights parse failed — using deterministic fallback.");
            return BuildFallbackInsights(snapshot, poweredByGemini: false);
        }

        return new GeminiInsightsResponse(
            NormalizeList(parsed.OperationalInsights, 5),
            NormalizeList(parsed.AnomalyHighlights, 4),
            NormalizeList(parsed.Recommendations, 5),
            string.IsNullOrWhiteSpace(parsed.ExecutiveSummary)
                ? BuildExecutiveSummary(snapshot)
                : parsed.ExecutiveSummary.Trim(),
            NormalizeList(parsed.RiskObservations, 4),
            PoweredByGemini: true);
    }

    private static GeminiInsightsResponse BuildFallbackInsights(
        OperationalFactsSnapshot snapshot,
        bool poweredByGemini)
    {
        var insights = new List<string>
        {
            $"Dominant operational risk posture: **{snapshot.DominantRiskLevel}**.",
            $"{snapshot.AnomalyCount} anomaly signal(s) in the selected window.",
            $"{snapshot.TransactionCount} fuel movement(s) available for correlation."
        };

        IReadOnlyList<string> highlights = snapshot.AnomalyCount > 0
            ? new[]
            {
                "One or more tanks show material variance between expected and actual flow.",
                "Investigate stations with HIGH or CRITICAL deterministic risk scores first."
            }
            : new[]
            {
                "No anomalies in the current 30-day fact window — maintain watch posture."
            };

        return new GeminiInsightsResponse(
            insights,
            highlights,
            DefaultRecommendations(snapshot),
            BuildExecutiveSummary(snapshot),
            [
                $"Risk level anchor: {snapshot.DominantRiskLevel}.",
                "Deterministic agents remain source of truth for scoring; AI intelligence layer adds narrative when configured."
            ],
            poweredByGemini,
            Disclaimer: poweredByGemini ? null : "AI intelligence layer offline — insights synthesized from live operational facts only.");
    }

    private static string BuildExecutiveSummary(OperationalFactsSnapshot snapshot) =>
        $"FuelGuard operational snapshot: {snapshot.AnomalyCount} anomalies and {snapshot.TransactionCount} " +
        $"transactions in scope; enterprise risk anchor at {snapshot.DominantRiskLevel}. " +
        "Prioritize field validation where variance and outbound spikes co-occur.";

    private static IReadOnlyList<string> DefaultRecommendations(OperationalFactsSnapshot snapshot)
    {
        if (snapshot.AnomalyCount == 0)
            return
            [
                "Maintain standard reconciliation cadence across all monitored stations.",
                "Review risk score drift weekly.",
                "Configure the AI intelligence API key for AI-assisted executive briefings."
            ];

        return
        [
            "Open targeted investigations on top-severity anomalies.",
            "Cross-check inventory dip vs telemetry for affected tanks.",
            "Brief compliance on sustained HIGH/CRITICAL risk posture.",
            "Schedule executive review if anomaly count increases week-over-week."
        ];
    }

    private static IReadOnlyList<string> NormalizeList(IEnumerable<string>? items, int max)
    {
        if (items is null)
            return Array.Empty<string>();

        return items
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Select(s => s.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Take(max)
            .ToList();
    }
}
