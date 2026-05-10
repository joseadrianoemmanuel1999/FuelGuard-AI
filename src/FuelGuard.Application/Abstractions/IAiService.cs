namespace FuelGuard.Application.Abstractions;

/// <summary>
/// Optional AI narrative layer. Implementations must never throw into the risk pipeline.
/// </summary>
public interface IAiService
{
    /// <summary>
    /// Produces investigator-facing explanation and actions. Returns null when AI is disabled or fails.
    /// </summary>
    Task<AIExplanationResponse?> ExplainRiskAsync(
        RiskAssessmentContext context,
        CancellationToken cancellationToken = default);
}
