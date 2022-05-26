using System.Diagnostics.CodeAnalysis;

namespace MockBot.Api.Models
{
    // No logic so no unit tests are required
    [ExcludeFromCodeCoverage]
    public class DmrMessage
    {
        public string MessageRefId { get; }

        public string SentBy { get; }

        public string SendTo { get; }

        public string Payload { get; }

        public DmrMessage(string messageRefId, string sentBy, string sendTo, string payload)
        {
            MessageRefId = messageRefId;
            SentBy = sentBy;
            SendTo = sendTo;
            Payload = payload;
        }
    }
}