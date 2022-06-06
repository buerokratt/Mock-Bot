using System.Collections.Concurrent;
using MockBot.Api.Interfaces;
using MockBot.Api.Models;

namespace MockBot.Api.Services
{
    public class ChatService : IChatService
    {
        private IDictionary<Guid, Chat> Chats { get; set; }
        public IDictionary<string, Message> DmrRequests { get; }

        public ChatService()
        {
            Chats = new ConcurrentDictionary<Guid, Chat>();
            DmrRequests = new ConcurrentDictionary<string, Message>();
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

        public Chat? FindById(Guid chatId)
        {
            return Chats.TryGetValue(chatId, out var chat) ? chat : null;
        }

        public Message? AddMessage(Guid chatId, string content)
        {
            var message = new Message(content);
            var chat = FindById(chatId);

            if (chat == null)
            {
                return null;
            }

            chat.Messages.Add(message);
            return message;
        }

        public void AddMessageMetadata(IHeaderDictionary headers)
        {
            if (headers == null)
            {
                return;
            }

            _ = headers.TryGetValue(Constants.MessageIdRefHeaderKey, out var messageIdRefHeader);
            _ = headers.TryGetValue(Constants.SentByHeaderKey, out var sentByHeader);
            _ = headers.TryGetValue(Constants.SendToHeaderKey, out var sendToHeader);
            _ = headers.TryGetValue(Constants.ModelTypeHeaderKey, out var modelTypeHeader);

            var message = DmrRequests[messageIdRefHeader];
            message.SentBy = sentByHeader;
            message.SendTo = sendToHeader;
            message.ModelType = modelTypeHeader;
        }

        public void AddDmrRequest(Message? message)
        {
            if (message == null)
            {
                return;
            }

            DmrRequests.Add(message.Id.ToString(), message);
        }
    }
}