using Buerokratt.Common.Models;
using MockBot.Api.Models;

namespace MockBot.Api.Interfaces
{
    /// <summary>
    /// This interface handles creating and retrieving chats and messages,
    /// maintaining a list of requests sent to DMR,
    /// using the DMR request list to update the appropriate message with received metadata
    /// </summary>
    public interface IChatService
    {
        public Chat CreateChat();

        public IEnumerable<Chat> FindAll();

        public Chat FindById(Guid chatId);

        public Chat FindByMessageId(Guid chatMessageId);

        public ChatMessage AddMessage(Guid chatId, string content, HeadersInput headers, string classification = default);

        public void AddDmrRequest(ChatMessage message);
    }
}