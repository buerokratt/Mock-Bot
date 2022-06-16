using System.Collections.Concurrent;
using MockBot.Api.Interfaces;
using MockBot.Api.Models;
using RequestProcessor.Models;
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

        public ChatMessage AddMessage(Guid chatId, string content)
        {
            var message = new ChatMessage(content);
            var chat = FindById(chatId);

            if (chat == null)
            {
                throw new ArgumentOutOfRangeException(nameof(chatId));
            }

            chat.Messages.Add(message);
            return message;
        }

        public void AddMessageMetadata(HeadersInput headers)
        {
            if (headers == null)
            {
                throw new ArgumentNullException(nameof(headers));
            };

            if (!DmrRequests.ContainsKey(headers.XMessageIdRef))
            {
                throw new ArgumentException(headers.XMessageIdRef);
            }

            var message = DmrRequests[headers.XMessageIdRef];
            message.SentBy = headers.XSentBy;
            message.SendTo = headers.XSendTo;
            message.ModelType = headers.XModelType;
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