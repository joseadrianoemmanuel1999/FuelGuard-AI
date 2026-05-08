using FuelGuard.Shared.Events;

namespace FuelGuard.Shared.Abstractions;

public interface IEventBus
{
    void Subscribe<TEvent>(Func<IServiceProvider, TEvent, CancellationToken, Task> handler)
        where TEvent : class, IEvent;

    Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : class, IEvent;
}
