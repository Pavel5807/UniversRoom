using UniversRoom.BuildingBlocks.EventBus.Events;

namespace UniversRoom.BuildingBlocks.EventBus.Abstractions;

public interface IEventBus
{
    void Publish(IntegrationEvent @event);

    void Subscribe<Event, EventHandler>()
        where Event : IntegrationEvent
        where EventHandler : IIntegrationEventHandler<Event>;

    void Unsubscribe<Event, EventHandler>()
        where Event : IntegrationEvent
        where EventHandler : IIntegrationEventHandler<Event>;
}
