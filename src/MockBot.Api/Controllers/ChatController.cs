using Microsoft.AspNetCore.Mvc;
using MockBot.Api.Interfaces;
using MockBot.Api.Models;

namespace MockBot.Api.Controllers
{
    [Route("api/chats")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpGet]
        public OkObjectResult FindAll()
        {
            var chats = _chatService.FindAll();

            return Ok(chats);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Chat))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult Get(Guid id)
        {
            var chat = _chatService.FindById(id);

            return chat == null ? NotFound() : Ok(chat);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Chat))]
        public CreatedResult Post()
        {
            var chat = _chatService.CreateChat();

            return Created(new Uri($"/chats/{chat.Id}", UriKind.Relative), chat);
        }

        [HttpPost("{chatId}/messages")]
        public CreatedResult PostMessage(Guid chatId, [FromBody] string content)
        {
            var message = _chatService.AddMessage(chatId, content);

            return Created(new Uri($"/chats/{chatId}/messages", UriKind.Relative), message);
        }
    }
}