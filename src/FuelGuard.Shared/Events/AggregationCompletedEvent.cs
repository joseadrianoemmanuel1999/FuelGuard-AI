namespace FuelGuard.Shared.Events;

public sealed record AggregationCompletedEvent(
    Guid CompanyId,
    Guid TankId,
    DateOnly SnapshotDate,
    decimal OpeningStock,
    decimal ClosingStock,
    decimal NetMovement) : EventBase;
