using FuelGuard.Shared.Models;

namespace FuelGuard.Api.Contracts;

public sealed record AnomalyResponse(
    Guid Id,
    Guid CompanyId,
    Guid TankId,
    DateOnly SnapshotDate,
    decimal ExpectedValue,
    decimal ActualValue,
    AnomalySeverity Severity,
    string Explanation,
    DateTimeOffset DetectedAt);

public sealed record RiskScoreResponse(
    Guid CompanyId,
    string CompanyName,
    decimal Score,
    RiskLevel Level,
    string Reason);
