using MockBot.Api.Models;

namespace MockBot.Api.Interfaces
{
    public interface IChatService
    {
        public Chat CreateChat();

        public IEnumerable<Chat> FindAll();
        Chat GetId(Guid id);
        Message AddMessage(Guid chatId, string content);
    }
}