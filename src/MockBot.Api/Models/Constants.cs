using System.Diagnostics.CodeAnalysis;

namespace MockBot.Api.Models
{
    // No logic so no unit tests are required
    [ExcludeFromCodeCoverage]
    internal static class Constants
    {
        internal const string Unknown = "Unknown";
        internal const string MessageAcknowledgementModelType = "application/vnd.mockbot.messageacknowledge+json;version=1";
    }
}
