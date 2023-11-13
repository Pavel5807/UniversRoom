using System;
using RabbitMQ.Client;

namespace UniversRoom.BuildingBlocks.EventBusRabbitMQ;

public interface IRabbitMQPersistentConnection : IDisposable
{
    public bool IsConnected { get; }
    public IModel CreateModel();
    public bool TryConnect();
}
