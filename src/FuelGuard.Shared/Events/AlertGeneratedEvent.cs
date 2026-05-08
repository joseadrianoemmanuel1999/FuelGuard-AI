using FuelGuard.Shared.Models;

namespace FuelGuard.Shared.Events;

public sealed record AlertGeneratedEvent(
    Guid CompanyId,
    decimal RiskScore,
    RiskLevel Level,
    string Message,
    Guid? RelatedAnomalyId) : EventBase;
