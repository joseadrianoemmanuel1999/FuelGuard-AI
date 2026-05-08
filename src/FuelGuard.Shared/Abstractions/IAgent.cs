namespace FuelGuard.Shared.Abstractions;

public interface IAgent
{
    string Name { get; }

    void Register(IEventBus eventBus);
}
