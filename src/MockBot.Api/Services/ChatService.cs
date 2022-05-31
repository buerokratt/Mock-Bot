using System.Collections.Concurrent;
using MockBot.Api.Interfaces;
using MockBot.Api.Models;

namespace MockBot.Api.Services
{
    public class ChatService : IChatService
    {
        private static IDictionary<Guid, Chat>? Chats { get; set; }
        public IDictionary<string, Message> DmrRequests { get; }

        public ChatService()
        {
            Chats = new ConcurrentDictionary<Guid, Chat>();
            DmrRequests = new ConcurrentDictionary<string, Message>();
        }

        public Chat CreateChat()
        {
            var chat = new Chat();

            Chats ??= new ConcurrentDictionary<Guid, Chat>();

            Chats.Add(chat.Id, chat);
            return chat;
        }

        public IEnumerable<Chat> FindAll()
        {
            Chats ??= new ConcurrentDictionary<Guid, Chat>();

            return Chats.Values.ToList();
        }

        public Chat? FindById(Guid chatId)
        {
            Chats ??= new ConcurrentDictionary<Guid, Chat>();

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

        public void AddMessageMetadata(string? xSentBy, string? xSendTo, string? xMessageId, string? xMessageIdRef)
        {
            if (xMessageIdRef == null)
            {
                return;
            }

            var message = DmrRequests[xMessageIdRef];
            message.SentBy = xSentBy;
            message.SendTo = xSendTo;
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