using FuelGuard.Application.Abstractions;

namespace FuelGuard.Infrastructure.Ai;

public sealed class NoOpAiService : IAiService
{
    public Task<string?> ExplainRiskUpdateAsync(RiskAssessmentContext context, CancellationToken cancellationToken = default) =>
        Task.FromResult<string?>(null);
}
