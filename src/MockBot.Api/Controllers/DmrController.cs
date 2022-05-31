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
        [Consumes("application/vnd.classifier.classification+json;version=1")]
        public AcceptedResult PostDmrMessage([FromHeader] HeadersInput headers)
        {
            _chatService.AddMessageMetadata(headers);
            return Accepted();
        }
    }
}