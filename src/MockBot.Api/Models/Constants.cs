using System.Diagnostics.CodeAnalysis;

namespace MockBot.Api.Models
{
    // No logic so no unit tests are required
    [ExcludeFromCodeCoverage]
    public static class Constants
    {
        public const string MessageIdHeaderKey = "X-Message-Id";
        public const string MessageIdRefHeaderKey = "X-Message-Id-Ref";
        public const string SendToHeaderKey = "X-Sent-By";
        public const string SentByHeaderKey = "X-Send-To";
        public const string ModelTypeHeaderKey = "X-Model-Type";
    }
}