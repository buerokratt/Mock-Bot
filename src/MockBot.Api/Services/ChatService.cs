using MockBot.Api.Interfaces;
using MockBot.Api.Models;

namespace MockBot.Api.Services
{
    public class ChatService : IChatService
    {
        private readonly Dictionary<Guid, Chat> _chats;

        public ChatService()
        {
            _chats = new Dictionary<Guid, Chat>();
        }

        public Chat CreateChat()
        {
            var chat = new Chat();
            _chats.Add(chat.Id, chat);
            return chat;
        }

        public IEnumerable<Chat> FindAll()
        {
            return _chats.Values.ToList();
        }

        public Chat FindById(Guid id)
        {
            return _chats[id];
        }

        public Message AddMessage(Guid chatId, string content)
        {
            var message = new Message(content);
            FindById(chatId).Messages.Add(message);
            return message;
        }
    }
}