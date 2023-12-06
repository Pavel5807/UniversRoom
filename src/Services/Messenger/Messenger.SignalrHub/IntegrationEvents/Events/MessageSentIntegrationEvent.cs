using UniversRoom.BuildingBlocks.EventBus.Events;

namespace UniversRoom.Services.Messenger.SignalrHub.IntegrationEvents;

public record MessageSentIntegrationEvent : IntegrationEvent
{
    public required string Content { get; init; }
}