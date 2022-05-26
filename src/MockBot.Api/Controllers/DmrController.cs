using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using MockBot.Api.Interfaces;
using MockBot.Api.Models;

namespace MockBot.Api.Controllers
{
    [Route("dmr-api/messages")]
    [ApiController]
    public class DmrController : ControllerBase
    {
        private readonly IDmrService _dmrService;

        public DmrController(IDmrService dmrService)
        {
            _dmrService = dmrService;
        }

        [HttpPost("dmr-response/async/{messageIdRef}")]
        [Consumes("application/x.classifier.classification+json;version=1")]
        public async Task<IActionResult> PostDmrMessageAsync(
            [Required][FromHeader(Name = "X-Sent-By")] string? XSentBy,
            [Required][FromHeader(Name = "X-Send-To")] string? XSendTo,
            [Required][FromHeader(Name = "X-Message-Id")] string? XMessageId,
            [Required][FromHeader(Name = "X-Message-Id-Ref")] string? XMessageIdRef
        )
        {
            await Task.Run(new Action(() => _dmrService.AddDmrMessage(XSentBy, XSendTo, XMessageId, XMessageIdRef))).ConfigureAwait(true);
            return Accepted();
        }

        [HttpPost("dmr-response/{messageIdRef}")]
        [Consumes("application/x.classifier.classification+json;version=1")]
        public AcceptedResult PostDmrMessage(
            [Required][FromHeader(Name = "X-Sent-By")] string? XSentBy,
            [Required][FromHeader(Name = "X-Send-To")] string? XSendTo,
            [Required][FromHeader(Name = "X-Message-Id")] string? XMessageId,
            [Required][FromHeader(Name = "X-Message-Id-Ref")] string? XMessageIdRef
        )
        {
            _dmrService.AddDmrMessage(XSentBy, XSendTo, XMessageId, XMessageIdRef);
            return Accepted();
        }
    }
}