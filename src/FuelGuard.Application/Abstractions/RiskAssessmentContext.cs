using FuelGuard.Shared.Models;

namespace FuelGuard.Application.Abstractions;

/// <summary>
/// Read-only facts passed to the AI after deterministic risk scoring has already been applied.
/// </summary>
public sealed record RiskAssessmentContext(
    Guid CompanyId,
    Guid? AnomalyId,
    Guid? TankId,
    string Severity,
    decimal ExpectedValue,
    decimal ActualValue,
    decimal PreviousScore,
    decimal NewScore,
    RiskLevel CurrentRiskLevel,
    string AnomalyExplanation);
