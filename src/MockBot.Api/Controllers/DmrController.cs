using Microsoft.AspNetCore.Mvc;
using MockBot.Api.Interfaces;
using MockBot.Api.Models;

namespace MockBot.Api.Controllers
{
    [Route("dmr-api")]
    [ApiController]
    public class DmrController : ControllerBase
    {
        private readonly IChatService _chatService;

        public DmrController(IChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpPost("dmr-response")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult PostDmrMessage(HeadersInput headers)
        {
            try
            {
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