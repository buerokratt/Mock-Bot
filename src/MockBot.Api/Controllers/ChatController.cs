using Microsoft.AspNetCore.Mvc;
using MockBot.Api.Interfaces;
using MockBot.Api.Models;

namespace MockBot.Api.Controllers;

[Route("api/chats")]
public class ChatController : Controller
{
    private readonly IChatService _chatService;

    public ChatController(IChatService chatService)
    {
        _chatService = chatService;
    }

    [HttpGet]
    public IEnumerable<Chat> FindAll()
    {
        return _chatService.FindAll();
    }

    [HttpGet("{id}")]
    public Chat Get(Guid id)
    {
        return _chatService.Get(id);
    }
    
    [HttpPost]
    public Chat Post()
    {
        return _chatService.Create();
    }
    
    [HttpGet("{chatId}/messages")]
    public List<Message> GetMessages(Guid chatId)
    {
        return _chatService.Get(chatId).Messages;
    }

    [HttpPost("{chatId}/messages")]
    public Message PostMessage(Guid chatId, [FromBody] string content)
    {
        return _chatService.CreateMessage(chatId, content);
    }
}