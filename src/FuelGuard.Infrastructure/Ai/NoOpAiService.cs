using FuelGuard.Application.Abstractions;

namespace FuelGuard.Infrastructure.Ai;

/// <summary>
/// Used when <c>AI:ApiKey</c> is not configured — deterministic pipeline still runs with template fallbacks.
/// </summary>
public sealed class NoOpAiService : IAiService
{
    public Task<AIExplanationResponse?> ExplainRiskAsync(
        RiskAssessmentContext context,
        CancellationToken cancellationToken = default) =>
        Task.FromResult<AIExplanationResponse?>(null);
}
