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

        [HttpPost("dmr-response/async/{messageRefId}")]
        [Consumes("application/x.classifier.classification+json;version=1")]
        public async Task<IActionResult> PostDmrMessageAsync([FromHeader] HeadersInput headers)
        {
            await Task.Run(new Action(() => _dmrService.AddDmrMessage(headers))).ConfigureAwait(true);
            return Accepted();
        }

        [HttpPost("dmr-response/{messageRefId}")]
        [Consumes("application/x.classifier.classification+json;version=1")]
        public AcceptedResult PostDmrMessage([FromHeader] HeadersInput headers)
        {
            _dmrService.AddDmrMessage(headers);
            return Accepted();
        }
    }
}