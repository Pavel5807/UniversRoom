using System.Threading.Tasks;
using UniversRoom.BuildingBlocks.EventBus.Events;

namespace UniversRoom.BuildingBlocks.EventBus.Abstractions;

public interface IIntegrationEventHandler<in T> : IIntegrationEventHandler
    where T : IntegrationEvent
{
    Task Handle(T @event);
}

public interface IIntegrationEventHandler
{
}
