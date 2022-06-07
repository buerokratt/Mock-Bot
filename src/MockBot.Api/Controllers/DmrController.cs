using Microsoft.AspNetCore.Mvc;
using MockBot.Api.Interfaces;

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
        public AcceptedResult PostDmrMessage()
        {
            _chatService.AddMessageMetadata(Request.Headers);
            return Accepted();
        }
    }
}