using System.Diagnostics.CodeAnalysis;

namespace MockBot.Api.Models
{
    // No logic so no unit tests are required
    [ExcludeFromCodeCoverage]
    public static class Constants
    {
        public const string Unknown = "Unknown";
        public const string MessageAcknowledgementModelType = "application/vnd.mockbot.messageacknowledge+json;version=1";
    }
}
