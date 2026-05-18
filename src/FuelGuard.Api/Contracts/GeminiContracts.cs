namespace FuelGuard.Api.Contracts;

public sealed record GeminiChatRequestDto(
    string Question,
    string? StationHint = null,
    string? PeriodHint = null,
    string? AdditionalContext = null);

public sealed record GeminiChatResponseDto(
    string Answer,
    string RiskLevel,
    IReadOnlyList<string> Recommendations,
    string Explainability,
    bool PoweredByGemini,
    string? Disclaimer = null);

public sealed record GeminiInsightsResponseDto(
    IReadOnlyList<string> OperationalInsights,
    IReadOnlyList<string> AnomalyHighlights,
    IReadOnlyList<string> Recommendations,
    string ExecutiveSummary,
    IReadOnlyList<string> RiskObservations,
    bool PoweredByGemini,
    string? Disclaimer = null);
