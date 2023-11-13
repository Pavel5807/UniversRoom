using Microsoft.AspNetCore.SignalR;
using UniversRoom.Services.Messenger.SignalrHub.Models;

namespace UniversRoom.Services.Messenger.SignalrHub.Hubs;

public class MessengerHub : Hub
{
    private readonly ILogger<MessengerHub> _logger;

    public MessengerHub(ILogger<MessengerHub> logger)
    {
        _logger = logger;
    }

    public async Task Broadcast(Message message)
    {
        _logger.Log(LogLevel.Information, $"");
        await Clients.All.SendAsync("Broadcast", message);
    }

    public override Task OnConnectedAsync()
    {
        _logger.Log(LogLevel.Information, $"Connected: " + Context.ConnectionId);
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.Log(LogLevel.Information, $"{Context.ConnectionId} Disconnected {exception?.Message}");
        return base.OnDisconnectedAsync(exception);
    }

}
