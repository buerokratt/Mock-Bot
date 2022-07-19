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
    [Route("client-api/chatters")]
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
                using StreamReader reader = new(Request.Body, Encoding.UTF8);
                string content = await reader.ReadToEndAsync().ConfigureAwait(false);

                if (string.IsNullOrEmpty(content))
                {
                    return new BadRequestObjectResult(Models.Constants.PostNoBodyMessage);
                }

                var headers = new HeadersInput()
                {
                    XSentBy = _settings.Id,
                    XSendTo = Models.Constants.XSendToDmr,
                    XModelType = Models.Constants.XModelType,
                };

                var message = _chatService.AddMessage(chatId, content, headers);
                _chatService.AddDmrRequest(message);

                _dmrService.Enqueue(GetDmrRequest(message, _settings.Id));
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
        private static DmrRequest GetDmrRequest(ChatMessage message, string botId, string classification = default)
        {
            // Setup headers
            var dmrHeaders = new HeadersInput
            {
                XSentBy = botId,
                XSendTo = Models.Constants.XSendToClassifier,
                XMessageId = message.Id.ToString(),
                XModelType = Models.Constants.XModelType,
                ContentType = Models.Constants.ContentTypePlain
            };

            // Setup payload
            var dmrPayload = new DmrRequestPayload()
            {
                Message = message.Content,
                Classification = string.IsNullOrEmpty(classification) ? string.Empty : classification
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