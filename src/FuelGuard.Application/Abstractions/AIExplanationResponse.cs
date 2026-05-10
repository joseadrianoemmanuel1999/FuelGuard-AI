namespace FuelGuard.Application.Abstractions;

/// <summary>
/// Structured fraud-analysis output from the AI explanation layer (never used to compute scores).
/// </summary>
public sealed record AIExplanationResponse(
    string Summary,
    IReadOnlyList<string> RecommendedActions,
    string Confidence);
