using FuelGuard.Shared.Models;

namespace FuelGuard.Shared.Events;

public sealed record RiskCalculatedEvent(
    Guid CompanyId,
    decimal Score,
    RiskLevel Level,
    Guid? TriggeringAnomalyId,
    string Reason) : EventBase;
