using Microsoft.AspNetCore.Mvc;
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

        public DmrController(IChatService chatService, IEncodingService encoder)
        {
            _chatService = chatService;
            _encoder = encoder;
        }

        [HttpPost("dmr-response")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> PostDmrMessageAsync([FromHeader] HeadersInput headers)
        {
            try
            {
                Console.WriteLine($"Received Message from {headers?.XSentBy}");

                string payload;
                using (StreamReader reader = new(Request.Body, Encoding.UTF8))
                {
                    payload = await reader.ReadToEndAsync().ConfigureAwait(false);
                }

                Console.WriteLine($"Received Message {payload}");
                var response = _encoder.DecodeBase64(payload);
                Console.WriteLine(response);

                _chatService.AddMessageMetadata(headers);
                return Accepted();
            }
            catch (ArgumentException)
            {
                return NotFound(headers?.XMessageIdRef ?? "Unknown");
            }
        }
    }
}