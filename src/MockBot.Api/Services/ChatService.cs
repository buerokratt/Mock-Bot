using MockBot.Api.Interfaces;
using MockBot.Api.Models;

namespace MockBot.Api.Services;

public class ChatService : IChatService
{
    private readonly Dictionary<Guid, Chat> _chats;

    public ChatService()
    {
        _chats = new Dictionary<Guid, Chat>();
    }

    public Chat Create()
    {
        var chat = new Chat();
        _chats.Add(chat.Id, chat);
        return chat;
    }

    public IEnumerable<Chat> FindAll()
    {
        return _chats.Values.ToList();
    }

    public Chat Get(Guid id)
    {
        return _chats[id];
    }

    public Message CreateMessage(Guid chatId, string content)
    {
        var message = new Message(content);
        Get(chatId).Messages.Add(message);
        return message;
    }
}