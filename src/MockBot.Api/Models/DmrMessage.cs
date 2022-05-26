using System.Diagnostics.CodeAnalysis;

namespace MockBot.Api.Models
{
    // No logic so no unit tests are required
    [ExcludeFromCodeCoverage]
    public class DmrMessage
    {
        public string MessageIdRef { get; }

        public string SentBy { get; }

        public string SendTo { get; }

        public string Payload { get; }

        public DmrMessage(string messageIdRef, string sentBy, string sendTo, string payload)
        {
            MessageIdRef = messageIdRef;
            SentBy = sentBy;
            SendTo = sendTo;
            Payload = payload;
        }
    }
}