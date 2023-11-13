using System;
using System.Collections.Generic;
using UniversRoom.BuildingBlocks.EventBus.Abstractions;
using UniversRoom.BuildingBlocks.EventBus.Events;
using static UniversRoom.BuildingBlocks.EventBus.InMemoryEventBusSubscriptionsManager;

namespace UniversRoom.BuildingBlocks.EventBus;

public interface IEventBusSubscriptionsManager
{
    public bool IsEmpty { get; }

    public event EventHandler<string> OnEventRemoved;

    public void AddDynamicSubscription<TH>(string eventName)
        where TH : IDynamicIntegrationEventHandler;

    public void AddSubscription<T, TH>()
        where T : IntegrationEvent
        where TH : IIntegrationEventHandler<T>;

    public void Clear();

    public string GetEventKey<T>();

    public Type? GetEventTypeByName(string eventName);

    public IEnumerable<SubscriptionInfo> GetHandlersForEvent<T>()
        where T : IntegrationEvent;

    public IEnumerable<SubscriptionInfo> GetHandlersForEvent(string eventName);

    public bool HasSubscriptionsForEvent<T>()
        where T : IntegrationEvent;

    public bool HasSubscriptionsForEvent(string eventName);

    public void RemoveDynamicSubscription<TH>(string eventName)
        where TH : IDynamicIntegrationEventHandler;

    public void RemoveSubscription<T, TH>()
        where T : IntegrationEvent
        where TH : IIntegrationEventHandler<T>;
}
