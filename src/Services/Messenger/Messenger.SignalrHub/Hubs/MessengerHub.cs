using Microsoft.AspNetCore.SignalR;

namespace UniversRoom.Services.Messenger.SignalrHub.Hubs;

public class MessengerHub : Hub
{
    private readonly ILogger<MessengerHub> _logger;

    public MessengerHub(ILogger<MessengerHub> logger)
    {
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, Context.User.Identity.Name);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, Context.User.Identity.Name);
        await base.OnDisconnectedAsync(exception);
    }

}
