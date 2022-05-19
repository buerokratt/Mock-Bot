using MockBot.Api.Models;

namespace MockBot.Api.Interfaces;

public interface IChatService
{
    public Chat Create();

    public IEnumerable<Chat> FindAll();
    Chat Get(Guid id);
    Message CreateMessage(Guid chatId, string content);
}