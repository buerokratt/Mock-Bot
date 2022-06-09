using Microsoft.AspNetCore.Mvc;
using MockBot.Api.Controllers.Extensions;
using MockBot.Api.Interfaces;
using MockBot.Api.Models;
using System.Text;

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

                // Just log telemetry for the DMR Callback at the moment...
                _logger.DmrCallbackReceived(headers?.XSentBy ?? "Unknown", encodedPayload, decodedPayload);

                _chatService.AddMessageMetadata(headers);
            }
            catch (ArgumentException)
            {
                return NotFound(headers?.XMessageIdRef ?? "Unknown");
            }

            return Accepted();
        }
    }
}