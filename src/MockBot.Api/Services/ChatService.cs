using Buerokratt.Common.Models;
using MockBot.Api.Interfaces;
using MockBot.Api.Models;
using System.Collections.Concurrent;
using Chat = MockBot.Api.Models.Chat;

namespace MockBot.Api.Services
{
    public class ChatService : IChatService
    {
        private IDictionary<Guid, Chat> Chats { get; set; }
        public IDictionary<string, ChatMessage> DmrRequests { get; }

        public ChatService()
        {
            Chats = new ConcurrentDictionary<Guid, Chat>();
            DmrRequests = new ConcurrentDictionary<string, ChatMessage>();
        }

        public Chat CreateChat()
        {
            var chat = new Chat();

            Chats.Add(chat.Id, chat);
            return chat;
        }

        public IEnumerable<Chat> FindAll()
        {
            return Chats.Values.ToList();
        }

        public Chat FindById(Guid chatId)
        {
            return Chats.TryGetValue(chatId, out var chat) ? chat : null;
        }

        public Chat FindByMessageId(Guid chatMessageId)
        {
            return Chats.Values.FirstOrDefault(c => c.Messages.Select(m => m.Id).Contains(chatMessageId));
        }

        public ChatMessage AddMessage(Guid chatId, string content, HeadersInput headers, string classification = default)
        {
            if (headers == null)
            {
                throw new ArgumentNullException(nameof(headers));
            };

            if (chatId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(chatId));
            };

            var message = new ChatMessage(content)
            {
                Classification = (classification == default) ? string.Empty : classification,
                SentBy = headers.XSentBy,
                SendTo = headers.XSendTo,
                ModelType = headers.XModelType
            };

            var chat = FindById(chatId);

            if (chat == null)
            {
                throw new ArgumentOutOfRangeException(nameof(chatId));
            }

            chat.Messages.Add(message);
            return message;
        }

        public void AddDmrRequest(ChatMessage message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            };

            DmrRequests.Add(message.Id.ToString(), message);
        }
    }
}