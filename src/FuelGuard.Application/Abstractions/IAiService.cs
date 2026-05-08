namespace FuelGuard.Application.Abstractions;

public interface IAiService
{
    Task<string?> ExplainRiskUpdateAsync(RiskAssessmentContext context, CancellationToken cancellationToken = default);
}

public sealed record RiskAssessmentContext(
    Guid CompanyId,
    Guid? AnomalyId,
    string Severity,
    string AnomalyExplanation,
    decimal PreviousScore,
    decimal NewScore);
