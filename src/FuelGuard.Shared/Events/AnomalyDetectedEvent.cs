using FuelGuard.Shared.Models;

namespace FuelGuard.Shared.Events;

public sealed record AnomalyDetectedEvent(
    Guid AnomalyId,
    Guid CompanyId,
    Guid TankId,
    DateOnly SnapshotDate,
    decimal ExpectedValue,
    decimal ActualValue,
    AnomalySeverity Severity,
    string Explanation) : EventBase;
