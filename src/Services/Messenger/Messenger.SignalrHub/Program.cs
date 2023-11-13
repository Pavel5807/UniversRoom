using UniversRoom.Services.Messenger.SignalrHub.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();

var app = builder.Build();

app.MapHub<MessengerHub>("/messenger");

app.Run();
