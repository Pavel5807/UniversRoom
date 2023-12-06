using UniversRoom.Services.Messenger.API.Models;

namespace UniversRoom.Services.Messenger.API.Repositories;

public class FakeRepository : IMessageRepository
{
    public void AddMessage(Message message)
    {
        throw new NotImplementedException();
    }

    public MessageInfo GetLastMessages(Guid userId)
    {
        throw new NotImplementedException();
    }

    public void Save()
    {
        throw new NotImplementedException();
    }
}
