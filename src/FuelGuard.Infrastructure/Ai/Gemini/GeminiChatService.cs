using FuelGuard.Application.Abstractions.Gemini;
using Microsoft.Extensions.Logging;

namespace FuelGuard.Infrastructure.Ai.Gemini;

public sealed class GeminiChatService(
    GeminiOperationalContextBuilder contextBuilder,
    GeminiClient client,
    ILogger<GeminiChatService> logger)
{
    public async Task<GeminiChatResponse> ChatAsync(
        GeminiChatRequest request,
        CancellationToken cancellationToken)
    {
        var snapshot = await contextBuilder.BuildAsync(
            request.StationHint,
            request.PeriodHint,
            cancellationToken);

        if (!client.IsConfigured)
            return BuildFallbackChat(request.Question, snapshot, poweredByGemini: false);

        var prompt = GeminiPromptBuilder.BuildChatPrompt(
            request.Question,
            snapshot.FactsBlock,
            request.StationHint,
            request.PeriodHint,
            request.AdditionalContext);

        var raw = await client.GenerateJsonAsync(prompt, cancellationToken);
        var parsed = GeminiJsonParser.ParseChat(raw);

        if (parsed is null || string.IsNullOrWhiteSpace(parsed.Answer))
        {
            logger.LogWarning("Gemini chat parse failed — using deterministic fallback.");
            return BuildFallbackChat(request.Question, snapshot, poweredByGemini: false);
        }

        var recommendations = NormalizeList(parsed.Recommendations, 6);
        if (recommendations.Count == 0)
            recommendations = DefaultRecommendations(snapshot);

        return new GeminiChatResponse(
            parsed.Answer.Trim(),
            NormalizeRiskLevel(parsed.RiskLevel, snapshot.DominantRiskLevel),
            recommendations,
            string.IsNullOrWhiteSpace(parsed.Explainability)
                ? BuildExplainability(snapshot)
                : parsed.Explainability.Trim(),
            PoweredByGemini: true);
    }

    private static GeminiChatResponse BuildFallbackChat(
        string question,
        OperationalFactsSnapshot snapshot,
        bool poweredByGemini)
    {
        var answer = snapshot.AnomalyCount > 0
            ? $"Based on {snapshot.AnomalyCount} recorded anomaly signal(s) and dominant risk **{snapshot.DominantRiskLevel}**, " +
              "operational review is recommended. Configure the AI intelligence API key for full narrative analysis."
            : "Telemetry appears nominal in the current fact window. No anomalies matched your filters.";

        return new GeminiChatResponse(
            $"{answer}\n\n**Your question:** {question.Trim()}",
            snapshot.DominantRiskLevel,
            DefaultRecommendations(snapshot),
            BuildExplainability(snapshot),
            poweredByGemini,
            Disclaimer: poweredByGemini ? null : "AI intelligence layer offline — showing deterministic operational summary.");
    }

    private static string BuildExplainability(OperationalFactsSnapshot snapshot) =>
        $"Analysis grounded in {snapshot.AnomalyCount} anomaly record(s), {snapshot.TransactionCount} transaction(s), " +
        $"and current dominant risk level {snapshot.DominantRiskLevel} from FuelGuard deterministic scoring.";

    private static IReadOnlyList<string> DefaultRecommendations(OperationalFactsSnapshot snapshot)
    {
        if (snapshot.AnomalyCount == 0)
            return
            [
                "Continue monitoring aggregate flow variance across active stations.",
                "Validate dip readings against telemetry for the next reporting window.",
                "Review risk scores after the next ingestion cycle."
            ];

        return
        [
            "Prioritize reconciliation at stations with highest severity anomalies.",
            "Dispatch field inspection for tanks with largest expected vs actual delta.",
            "Escalate to compliance if variance persists beyond two consecutive windows.",
            "Correlate outbound transactions with anomaly timestamps."
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

    private static string NormalizeRiskLevel(string? level, string fallback)
    {
        var normalized = string.IsNullOrWhiteSpace(level) ? fallback : level.Trim().ToUpperInvariant();
        return normalized is "LOW" or "MEDIUM" or "HIGH" or "CRITICAL" or "UNKNOWN"
            ? normalized
            : fallback;
    }
}
