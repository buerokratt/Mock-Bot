using Buerokratt.Common.AsyncProcessor;
using Buerokratt.Common.Dmr;
using Buerokratt.Common.Encoder;
using Buerokratt.Common.Models;
using Microsoft.AspNetCore.Mvc;
using MockBot.Api.Configuration;
using MockBot.Api.Controllers.Extensions;
using MockBot.Api.Interfaces;
using System.Text;
using System.Text.Json;

namespace MockBot.Api.Controllers
{
    [Route("dmr-api")]
    [ApiController]
    public class DmrController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly IEncodingService _encoder;
        private readonly IAsyncProcessorService<DmrRequest> _processor;
        private readonly BotSettings _settings;
        private readonly ILogger<DmrController> _logger;

        public DmrController(
            IChatService chatService,
            IEncodingService encoder,
            IAsyncProcessorService<DmrRequest> processor,
            BotSettings settings,
            ILogger<DmrController> logger)
        {
            _chatService = chatService;
            _encoder = encoder;
            _processor = processor;
            _settings = settings;
            _logger = logger;
        }

        [HttpPost("messages")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> PostDmrMessageAsync([FromHeader] HeadersInput headers)
        {
            try
            {
                string encodedPayload;
                using (StreamReader reader = new(Request.Body, Encoding.UTF8))
                {
                    encodedPayload = await reader.ReadToEndAsync().ConfigureAwait(false);
                }

                var decodedPayload = _encoder.DecodeBase64(encodedPayload);
                var payload = JsonSerializer.Deserialize<DmrRequestPayload>(decodedPayload);

                // Add the message to the chat
                var chat = _chatService.FindByMessageId(new Guid(headers?.XMessageIdRef));
                if (chat == null)
                {
                    // No matching message, create a new chat and log a warning
                    chat = _chatService.CreateChat();
                    _logger.MessageWithNoChatReceived(headers?.XMessageIdRef, chat.Id.ToString(), encodedPayload, decodedPayload);
                }

                _ = _chatService.AddMessage(chat.Id, payload.Message, headers, payload.Classification);

                if (headers.XModelType == ModelTypes.MessageRequest)
                {
                    SendBackAcknowledgement(headers, chat.Id, payload.Message);
                }

                // Log telemtary
                _logger.DmrCallbackReceived(
                    headers?.XSentBy ?? Models.Constants.Unknown,
                    headers?.XMessageIdRef ?? Models.Constants.Unknown,
                    encodedPayload,
                    decodedPayload);
            }
            catch (ArgumentException)
            {
                return NotFound(headers?.XMessageIdRef ?? Models.Constants.Unknown);
            }

            return Accepted();
        }

        private void SendBackAcknowledgement(HeadersInput incomingHeaders, Guid chatId, string incomingMessageContent)
        {
            var messageContent = $"Acknowledging message: '{incomingMessageContent}'";

            var headers = new HeadersInput
            {
                XSentBy = _settings.Id,
                XSendTo = incomingHeaders.XSentBy, // This is "SentBy" because the sender is now the recipient
                XMessageIdRef = incomingHeaders.XMessageId,
                XModelType = ModelTypes.MessageAcknowledgement,
                ContentType = incomingHeaders.ContentType
            };

            var message = _chatService.AddMessage(chatId, messageContent, headers);

            headers.XMessageId = message.Id.ToString();

            var dmrRequest = new DmrRequest(headers)
            {
                Payload = new DmrRequestPayload
                {
                    Message = messageContent
                }
            };

            _processor.Enqueue(dmrRequest);

        }
    }
}