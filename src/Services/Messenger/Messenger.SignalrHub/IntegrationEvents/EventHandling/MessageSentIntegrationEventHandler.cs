using Microsoft.AspNetCore.SignalR;
using UniversRoom.BuildingBlocks.EventBus.Abstractions;
using UniversRoom.Services.Messenger.SignalrHub.Hubs;

namespace UniversRoom.Services.Messenger.SignalrHub.IntegrationEvents;

public class MessageSentIntegrationEventHandler : IIntegrationEventHandler<MessageSentIntegrationEvent>
{
    private readonly IHubContext<MessengerHub> _hubContext;
    private readonly ILogger<MessageSentIntegrationEventHandler> _logger;

    public MessageSentIntegrationEventHandler(
        IHubContext<MessengerHub> hubContext,
        ILogger<MessageSentIntegrationEventHandler> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task Handle(MessageSentIntegrationEvent @event)
    {
        await _hubContext.Clients.All.SendAsync("UpdateMessage", new { @event.Content, @event.CreationDate });
    }
}