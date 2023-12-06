using UniversRoom.Services.Messenger.API.Models;

namespace UniversRoom.Services.Messenger.API.Repositories;

public interface IMessageRepository
{
    void AddMessage(Message message);
    MessageInfo GetLastMessages(Guid userId);
    void Save();
}
