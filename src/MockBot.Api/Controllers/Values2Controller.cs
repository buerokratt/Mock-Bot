using Microsoft.AspNetCore.Mvc;

// This is a sample controller used only for the purposes of having some code to test until the actual solution gets built
// Without this, we could not pass the code coverage requirements for the CI pipeline
// This should be removed when actual solution code gets added

namespace MockBot.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Values2Controller : ControllerBase
    {
        // GET: api/<Values2Controller>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }
    }
}
