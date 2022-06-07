using Microsoft.AspNetCore.Mvc;
using MockBot.Api.Interfaces;
using MockBot.Api.Models;
using MockBot.Api.Services.Dmr;

namespace MockBot.Api.Controllers
{
    [Route("client-api/chats")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly IDmrService _dmrService;

        public ChatController(IChatService chatService, IDmrService dmrService)
        {
            _chatService = chatService;
            _dmrService = dmrService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public OkObjectResult FindAll()
        {
            var chats = _chatService.FindAll();

            return Ok(chats);
        }

        [HttpGet("{chatId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Chat> Get(Guid chatId)
        {
            var chat = _chatService.FindById(chatId);

            return chat == null ? NotFound() : chat;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Chat))]
        public CreatedResult Post()
        {
            var chat = _chatService.CreateChat();

            return Created(new Uri($"/chats/{chat.Id}", UriKind.Relative), chat);
        }

        [HttpPost("{chatId}/messages")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Message))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult PostMessage(Guid chatId, [FromBody] string content)
        {
            try
            {
                var message = _chatService.AddMessage(chatId, content);
                _chatService.AddDmrRequest(message);

                _dmrService.RecordRequest(GetDmrRequest(message, ""));
                return Created(new Uri($"/chats/{chatId}/messages", UriKind.Relative), message);
            }
            catch (ArgumentOutOfRangeException)
            {
                return NotFound(chatId);
            }
        }

        /// <summary>
        /// Builds a DmrRequest object from parameters
        /// </summary>
        /// <param name="message">The message to go into the .Payload.Message property</param>
        /// <param name="classification">No classification before getting send to DMR</param>
        /// <returns>A DmrRequest object</returns>
        private static DmrRequest GetDmrRequest(Message message, string classification)
        {
            // Setup headers
            var dmrHeaders = new HeadersInput
            {
                XSentBy = "MockBot",
                XSendTo = "Classifier",
                XMessageId = message.Id.ToString(),
                XModelType = "application/vnd.classifier.classification+json;version=1",
                ContentType = "text/plain"
            };

            // Setup payload
            var dmrPayload = new DmrRequestPayload()
            {
                Message = message.Content,
                Classification = classification
            };

            // Setup request
            var dmrRequest = new DmrRequest(dmrHeaders)
            {
                Payload = dmrPayload
            };

            return dmrRequest;
        }
    }
}