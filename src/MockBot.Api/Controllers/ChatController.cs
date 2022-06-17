using Microsoft.AspNetCore.Mvc;
using MockBot.Api.Configuration;
using MockBot.Api.Interfaces;
using MockBot.Api.Models;
using RequestProcessor.AsyncProcessor;
using RequestProcessor.Dmr;
using RequestProcessor.Models;
using System.Text;

namespace MockBot.Api.Controllers
{
    [Route("client-api/chats")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly IAsyncProcessorService<DmrRequest> _dmrService;
        private readonly BotSettings _settings;

        public ChatController(IChatService chatService, IAsyncProcessorService<DmrRequest> dmrService, BotSettings settings)
        {
            _chatService = chatService;
            _dmrService = dmrService;
            _settings = settings;
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
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> PostMessageAsync(Guid chatId)
        {
            try
            {
                string content;
                using (StreamReader reader = new(Request.Body, Encoding.UTF8))
                {
                    content = await reader.ReadToEndAsync().ConfigureAwait(false);
                }

                var message = _chatService.AddMessage(chatId, content);
                _chatService.AddDmrRequest(message);

                _dmrService.Enqueue(GetDmrRequest(message, string.Empty, _settings.Id));
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
        private static DmrRequest GetDmrRequest(ChatMessage message, string classification, string botId)
        {
            // Setup headers
            var dmrHeaders = new HeadersInput
            {
                XSentBy = botId,
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