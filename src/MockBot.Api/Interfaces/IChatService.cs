using MockBot.Api.Models;

namespace MockBot.Api.Interfaces
{
    public interface IChatService
    {
        public Chat CreateChat();

        public IEnumerable<Chat> FindAll();

        Chat? FindById(Guid chatId);

        Message? AddMessage(Guid chatId, string content);

        public void AddMessageMetadata(Message message, string? XSentBy, string? XSendTo, string? XMessageId, string? XMessageIdRef);
    }
}