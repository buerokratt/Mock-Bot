using MockBot.Api.Models;

namespace MockBot.Api.Interfaces
{
    /// <summary>
    /// This class handles creating and retrieving chats and messages,
    /// maintaining a list of requests sent to DMR,
    /// using the DMR request list to update the appropriate message with received metadata
    /// </summary>
    public interface IChatService
    {
        public Chat CreateChat();

        public IEnumerable<Chat> FindAll();

        public Chat? FindById(Guid chatId);

        public void AddDmrRequest(Message message);

        public Message? AddMessage(Guid chatId, string content);

        public void AddMessageMetadata(string? xSentBy, string? xSendTo, string? xMessageId, string? xMessageIdRef);
    }
}