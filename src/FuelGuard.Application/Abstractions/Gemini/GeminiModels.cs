namespace FuelGuard.Application.Abstractions.Gemini;

public sealed record GeminiChatRequest(
    string Question,
    string? StationHint = null,
    string? PeriodHint = null,
    string? AdditionalContext = null);

public sealed record GeminiChatResponse(
    string Answer,
    string RiskLevel,
    IReadOnlyList<string> Recommendations,
    string Explainability,
    bool PoweredByGemini,
    string? Disclaimer = null);

public sealed record GeminiInsightsResponse(
    IReadOnlyList<string> OperationalInsights,
    IReadOnlyList<string> AnomalyHighlights,
    IReadOnlyList<string> Recommendations,
    string ExecutiveSummary,
    IReadOnlyList<string> RiskObservations,
    bool PoweredByGemini,
    string? Disclaimer = null);

public sealed record OperationalFactsSnapshot(
    string FactsBlock,
    string DominantRiskLevel,
    int AnomalyCount,
    int TransactionCount);
