using FuelGuard.Shared.Abstractions;
using FuelGuard.Shared.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FuelGuard.Infrastructure.Events;

public sealed class InMemoryEventBus(IServiceScopeFactory scopeFactory, ILogger<InMemoryEventBus> logger)
    : IEventBus
{
    private readonly object _sync = new();
    private readonly Dictionary<Type, List<Delegate>> _handlers = new();

    public void Subscribe<TEvent>(Func<IServiceProvider, TEvent, CancellationToken, Task> handler)
        where TEvent : class, IEvent
    {
        ArgumentNullException.ThrowIfNull(handler);

        lock (_sync)
        {
            var key = typeof(TEvent);
            if (!_handlers.TryGetValue(key, out var list))
            {
                list = [];
                _handlers[key] = list;
            }

            list.Add(handler);
        }
    }

    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : class, IEvent
    {
        ArgumentNullException.ThrowIfNull(@event);

        List<Delegate> snapshot;
        lock (_sync)
        {
            if (!_handlers.TryGetValue(typeof(TEvent), out var list) || list.Count == 0)
            {
                logger.LogDebug("No subscribers for {EventType}", typeof(TEvent).Name);
                return;
            }

            snapshot = [..list];
        }

        logger.LogInformation(
            "Publishing {EventType} {EventId} (Correlation {CorrelationId}, {Timestamp}) to {HandlerCount} handler(s)",
            typeof(TEvent).Name,
            @event.EventId,
            @event.CorrelationId,
            @event.Timestamp,
            snapshot.Count);

        foreach (var handler in snapshot)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var typed = (Func<IServiceProvider, TEvent, CancellationToken, Task>)handler;
            await using var scope = scopeFactory.CreateAsyncScope();
            await typed(scope.ServiceProvider, @event, cancellationToken);
        }
    }
}
