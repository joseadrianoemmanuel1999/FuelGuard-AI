namespace FuelGuard.Shared.Events;

public interface IEvent
{
    Guid EventId { get; }
    Guid CorrelationId { get; }
    DateTimeOffset Timestamp { get; }
}
