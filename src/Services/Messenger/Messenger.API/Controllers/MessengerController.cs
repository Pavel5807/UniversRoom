using Microsoft.AspNetCore.Mvc;
using UniversRoom.BuildingBlocks.EventBus.Abstractions;
using UniversRoom.Services.Messenger.API.IntegrationEvents.Events;
using UniversRoom.Services.Messenger.API.Models;
using UniversRoom.Services.Messenger.API.Repositories;
using UniversRoom.Services.Messenger.API.Services;

namespace UniversRoom.Services.Messenger.API.Controllers;

[Route("api/v1/[Controller]")]
[ApiController]
public class MessengerController : ControllerBase
{
    private readonly IMessageRepository _messageRepository;
    private readonly IIdentityService _identityService;
    private readonly IEventBus _eventBus;

    public MessengerController(IMessageRepository messageRepository, IIdentityService identityService, IEventBus eventBus)
    {
        _messageRepository = messageRepository;
        _identityService = identityService;
        _eventBus = eventBus;
    }

    [HttpGet]
    public IActionResult GetFirstMessages()
    {
        var userId = _identityService.GetUserIdentity();
        var messageInfo = _messageRepository.GetLastMessages(userId);
        return Ok(messageInfo);
    }

    [HttpPost]
    public IActionResult SendMessage(Message message)
    {
        if (string.IsNullOrEmpty(message.Content))
        {
            return BadRequest();
        }

        _messageRepository.AddMessage(message);
        _messageRepository.Save();
        
        var messageSentEvent = new MessageSentIntegrationEvent();
        _eventBus.Publish(messageSentEvent);

        return Ok();
    }
}
