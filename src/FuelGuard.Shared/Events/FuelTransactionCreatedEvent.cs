namespace FuelGuard.Shared.Events;

public sealed record FuelTransactionCreatedEvent(
    Guid TransactionId,
    Guid CompanyId,
    Guid TankId,
    decimal Quantity,
    string MovementType,
    DateTimeOffset TransactionDate) : EventBase;
