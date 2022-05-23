using Microsoft.AspNetCore.Mvc;
using MockBot.Api.Interfaces;
using MockBot.Api.Models;

namespace MockBot.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
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

        [HttpGet("{id:guid}")]
        public Chat Get(Guid id)
        {
            return _chatService.GetId(id);
        }

        [HttpPost]
        public Chat Post()
        {
            return _chatService.CreateChat();
        }

        [HttpGet("{chatId:guid}/messages")]
        public IEnumerable<Message> GetMessages(Guid chatId)
        {
            return _chatService.GetId(chatId).Messages;
        }

        [HttpPost("{chatId:guid}/messages")]
        public Message PostMessage(Guid chatId, [FromBody] string content)
        {
            return _chatService.AddMessage(chatId, content);
        }
    }
}