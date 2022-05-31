using System.ComponentModel.DataAnnotations;
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
        public AcceptedResult PostDmrMessage(
            [Required][FromHeader(Name = "X-Sent-By")] string? xSentBy,
            [Required][FromHeader(Name = "X-Send-To")] string? xSendTo,
            [Required][FromHeader(Name = "X-Message-Id")] string? xMessageId,
            [Required][FromHeader(Name = "X-Message-Id-Ref")] string? xMessageIdRef
        )
        {
            _chatService.AddMessageMetadata(xSentBy, xSendTo, xMessageId, xMessageIdRef);
            return Accepted();
        }
    }
}