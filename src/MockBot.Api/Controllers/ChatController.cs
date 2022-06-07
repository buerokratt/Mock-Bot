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
        public CreatedResult PostMessage(Guid chatId, [FromBody] string content)
        {
            var message = _chatService.AddMessage(chatId, content);

            _dmrService.RecordRequest(GetDmrRequest(content, "", Request.Headers));
            return Created(new Uri($"/chats/{chatId}/messages", UriKind.Relative), message);
        }

        /// <summary>
        /// Builds a DmrRequest object from parameters
        /// </summary>
        /// <param name="message">The message to go into the .Payload.Message property</param>
        /// <param name="classification">No classification before getting send to DMR</param>
        /// <param name="headers">The header collection from the original Request. Used to create the .Header object</param>
        /// <returns>A DmrRequest object</returns>
        private static DmrRequest GetDmrRequest(string message, string classification, IHeaderDictionary headers)
        {
            // Setup headers
            _ = headers.TryGetValue(Constants.SentByHeaderKey, out var sentByHeader);
            _ = headers.TryGetValue(Constants.MessageIdHeaderKey, out var messageIdHeader);
            _ = headers.TryGetValue(Constants.SendToHeaderKey, out var sendToHeader);
            _ = headers.TryGetValue(Constants.MessageIdRefHeaderKey, out var messageIdRefHeader);
            _ = headers.TryGetValue(Constants.ModelTypeHeaderKey, out var modelTypeHeader);
            var dmrHeaders = new Dictionary<string, string>
            {
                { Constants.SentByHeaderKey, sendToHeader },
                { Constants.MessageIdHeaderKey, messageIdHeader },
                { Constants.SendToHeaderKey, sentByHeader },
                { Constants.MessageIdRefHeaderKey, messageIdRefHeader },
                { Constants.ModelTypeHeaderKey, modelTypeHeader }
            };

            // Setup payload
            var dmrPayload = new DmrRequestPayload()
            {
                Message = message,
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