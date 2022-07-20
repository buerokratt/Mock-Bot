using Buerokratt.Common.Dmr;
using Buerokratt.Common.Encoder;
using Buerokratt.Common.Models;
using Microsoft.AspNetCore.Mvc;
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
        private readonly ILogger<DmrController> _logger;

        public DmrController(IChatService chatService, IEncodingService encoder, ILogger<DmrController> logger)
        {
            _chatService = chatService;
            _encoder = encoder;
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
    }
}