using MockBot.Api.Models;
using RequestProcessor.Models;

namespace MockBot.Api.Interfaces
{
    /// <summary>
    /// This interface handles creating and retrieving chats and messages,
    /// maintaining a list of requests sent to DMR,
    /// using the DMR request list to update the appropriate message with received metadata
    /// </summary>
    public interface IChatService
    {
        public Models.Chat CreateChat();

        public IEnumerable<Models.Chat> FindAll();

        public Models.Chat FindById(Guid chatId);

        public ChatMessage AddMessage(Guid chatId, string content);

        public void AddDmrRequest(ChatMessage message);

        public void AddMessageMetadata(HeadersInput headers);
    }
}