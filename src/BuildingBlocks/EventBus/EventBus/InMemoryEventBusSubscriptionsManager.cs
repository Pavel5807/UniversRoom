using System;
using System.Collections.Generic;
using System.Linq;
using UniversRoom.BuildingBlocks.EventBus.Abstractions;
using UniversRoom.BuildingBlocks.EventBus.Events;

namespace UniversRoom.BuildingBlocks.EventBus;

public partial class InMemoryEventBusSubscriptionsManager : IEventBusSubscriptionsManager
{
    private readonly Dictionary<string, List<SubscriptionInfo>> _handlers;
    private readonly List<Type> _eventTypes;

    public InMemoryEventBusSubscriptionsManager()
    {
        _handlers = new Dictionary<string, List<SubscriptionInfo>>();
        _eventTypes = new List<Type>();
    }

    public bool IsEmpty => _handlers.Count is 0;

    public event EventHandler<string> OnEventRemoved;

    public void AddDynamicSubscription<TH>(string eventName)
        where TH : IDynamicIntegrationEventHandler
    {
        DoAddSubscription(typeof(TH), eventName, isDynamic: true);
    }

    public void AddSubscription<T, TH>()
        where T : IntegrationEvent
        where TH : IIntegrationEventHandler<T>
    {
        var eventName = GetEventKey<T>();

        DoAddSubscription(typeof(TH), eventName, isDynamic: false);

        if (_eventTypes.Contains(typeof(T)) is false)
        {
            _eventTypes.Add(typeof(T));
        }
    }

    public void Clear()
    {
        _handlers.Clear();
    }

    public string GetEventKey<T>()
    {
        return typeof(T).Name;
    }

    public Type? GetEventTypeByName(string eventName)
    {
        return _eventTypes.SingleOrDefault(t => t.Name == eventName);
    }

    public IEnumerable<SubscriptionInfo> GetHandlersForEvent<T>() where T : IntegrationEvent
    {
        var key = GetEventKey<T>();
        return GetHandlersForEvent(key);
    }

    public IEnumerable<SubscriptionInfo> GetHandlersForEvent(string eventName)
    {
        return _handlers[eventName];
    }

    public bool HasSubscriptionsForEvent<T>() 
        where T : IntegrationEvent
    {
        var key = GetEventKey<T>();
        return HasSubscriptionsForEvent(key);
    }

    public bool HasSubscriptionsForEvent(string eventName)
    {
        return _handlers.ContainsKey(eventName);
    }

    public void RemoveDynamicSubscription<TH>(string eventName)
        where TH : IDynamicIntegrationEventHandler
    {
        var handlerToRemove = FindDynamicSubscriptionToRemove<TH>(eventName);
        DoRemoveHandler(eventName, handlerToRemove);
    }

    public void RemoveSubscription<T, TH>()
        where T : IntegrationEvent
        where TH : IIntegrationEventHandler<T>
    {
        var eventName = GetEventKey<T>();
        var handlerToRemove = FindSubscriptionToRemove<T, TH>();
        DoRemoveHandler(eventName, handlerToRemove);
    }

    private void DoAddSubscription(Type handlerType, string eventName, bool isDynamic)
    {
        if (HasSubscriptionsForEvent(eventName) is false)
        {
            _handlers.Add(eventName, new List<SubscriptionInfo>());
        }

        if (_handlers[eventName].Any(s => s.HandlerType == handlerType))
        {
            throw new ArgumentException($"Handler Type {handlerType.Name} already registered for '{eventName}'", nameof(handlerType));
        }

        if (isDynamic)
        {
            _handlers[eventName].Add(SubscriptionInfo.Dynamic(handlerType));
        }
        else
        {
            _handlers[eventName].Add(SubscriptionInfo.Typed(handlerType));
        }
    }

    private SubscriptionInfo? DoFindSubscriptionToRemove(string eventName, Type handlerType)
    {
        if (HasSubscriptionsForEvent(eventName) is false)
        {
            return null;
        }

        return _handlers[eventName].SingleOrDefault(s => s.HandlerType == handlerType);
    }

    private void DoRemoveHandler(string eventName, SubscriptionInfo? subscription)
    {
        if (subscription is null)
        {
            return;
        }

        _handlers[eventName].Remove(subscription);
        if (_handlers[eventName].Any())
        {
            return;
        }

        _handlers.Remove(eventName);
        var eventType = _eventTypes.SingleOrDefault(e => e.Name == eventName);
        if (eventType is not null)
        {
            _eventTypes.Remove(eventType);
        }
        RaiseOnEventRemoved(eventName);

    }

    private SubscriptionInfo? FindDynamicSubscriptionToRemove<TH>(string eventName) 
        where TH : IDynamicIntegrationEventHandler
    {
        return DoFindSubscriptionToRemove(eventName, typeof(TH));
    }

    private SubscriptionInfo? FindSubscriptionToRemove<T, TH>()
        where T : IntegrationEvent
        where TH : IIntegrationEventHandler<T>
    {
        var eventName = GetEventKey<T>();
        return DoFindSubscriptionToRemove(eventName, typeof(TH));
    }

    private void RaiseOnEventRemoved(string eventName)
    {
        var handler = OnEventRemoved;
        handler?.Invoke(this, eventName);
    }
}
