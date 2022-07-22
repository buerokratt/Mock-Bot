using System.Diagnostics.CodeAnalysis;

namespace MockBot.Api.Controllers
{
    // No logic so no unit tests are required
    [ExcludeFromCodeCoverage]
    public static class Errors
    {
        public static readonly string PostNoBodyMessage = "Post must have a body";
    }
}
