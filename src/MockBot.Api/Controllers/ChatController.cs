using Buerokratt.Common.AsyncProcessor;
using Buerokratt.Common.Dmr;
using Buerokratt.Common.Models;
using Microsoft.AspNetCore.Mvc;
using MockBot.Api.Configuration;
using MockBot.Api.Interfaces;
using MockBot.Api.Models;
using System.Net.Mime;
using System.Text;

namespace MockBot.Api.Controllers
{
    [Route("client-api/chats")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly IAsyncProcessorService<DmrRequest> _processor;
        private readonly BotSettings _settings;

        public ChatController(IChatService chatService, IAsyncProcessorService<DmrRequest> dmrProcessorService, BotSettings settings)
        {
            _chatService = chatService;
            _processor = dmrProcessorService;
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
                    return BadRequest(Errors.PostNoBodyMessage);
                }

                var headers = GetHeadersInput(Request.Headers);

                var message = _chatService.AddMessage(chatId, content, headers);

                var dmrRequest = GetDmrRequest(message, headers);
                _processor.Enqueue(dmrRequest);

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
        private static DmrRequest GetDmrRequest(ChatMessage message, HeadersInput headers, string classification = default)
        {
            // Setup payload
            var dmrPayload = new DmrRequestPayload()
            {
                Message = message.Content,
                Classification = string.IsNullOrEmpty(classification) ? string.Empty : classification
            };

            headers.XMessageId = message.Id.ToString();

            // Setup request
            var dmrRequest = new DmrRequest(headers)
            {
                Payload = dmrPayload
            };

            return dmrRequest;
        }

        private HeadersInput GetHeadersInput(IHeaderDictionary headers)
        {
            var input = new HeadersInput
            {
                XSentBy = _settings.Id,
                XSendTo = headers[HeaderNames.XSendToHeaderName],
                XMessageId = headers[HeaderNames.XMessageIdHeaderName],
                XMessageIdRef = headers[HeaderNames.XMessageIdRefHeaderName],
                XModelType = headers[HeaderNames.XModelTypeHeaderName],
                ContentType = headers[HeaderNames.ContentTypeHeaderName]
            };

            if (string.IsNullOrEmpty(input.XSendTo))
            {
                input.XSendTo = ParticipantIds.ClassifierId;
                input.XModelType = ModelTypes.ClassificationRequest;
            }

            if (string.IsNullOrEmpty(input.ContentType))
            {
                input.ContentType = MediaTypeNames.Text.Plain;
            }

            return input;
        }
    }
}