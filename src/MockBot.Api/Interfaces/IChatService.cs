using MockBot.Api.Models;

namespace MockBot.Api.Interfaces
{
    public interface IChatService
    {
        public Chat CreateChat();

        public IEnumerable<Chat> FindAll();

        public Chat? FindById(Guid chatId);

        public void AddDmrRequest(Message message);

        public void AddDmrMessage(string? xSentBy, string? xSendTo, string? xMessageId, string? xMessageIdRef);

        public Message? AddMessage(Guid chatId, string content);

        public void AddMessageMetadata(string? xSentBy, string? xSendTo, string? xMessageId, string? xMessageIdRef);
    }
}