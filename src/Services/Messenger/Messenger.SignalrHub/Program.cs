using UniversRoom.BuildingBlocks.EventBus.Abstractions;
using UniversRoom.Services.Common;
using UniversRoom.Services.Messenger.SignalrHub.Hubs;
using UniversRoom.Services.Messenger.SignalrHub.IntegrationEvents;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddSignalR();

builder.Services.AddTransient<IIntegrationEventHandler<MessageSentIntegrationEvent>, MessageSentIntegrationEventHandler>();

var app = builder.Build();

app.MapHub<MessengerHub>("hub/messenger");

var eventBus = app.Services.GetRequiredService<IEventBus>();

eventBus.Subscribe<MessageSentIntegrationEvent, IIntegrationEventHandler<MessageSentIntegrationEvent>>();

app.Run();
