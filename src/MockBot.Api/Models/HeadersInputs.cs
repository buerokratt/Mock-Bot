using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;

namespace MockBot.Api.Models
{
    // No logic so no unit tests are required
    [ExcludeFromCodeCoverage]
    public class HeadersInput
    {
        [FromHeader(Name = Constants.SentByHeaderKey)]
        public string XSentBy { get; set; }

        [FromHeader(Name = Constants.SendToHeaderKey)]
        public string XSendTo { get; set; }

        [FromHeader(Name = Constants.MessageIdHeaderKey)]
        public string XMessageId { get; set; }

        [FromHeader(Name = Constants.MessageIdRefHeaderKey)]
        public string XMessageIdRef { get; set; }

        [FromHeader(Name = Constants.ModelTypeHeaderKey)]
        public string XModelType { get; set; }

        [FromHeader(Name = Constants.ContentTypeHeaderKey)]
        public string ContentType { get; set; }
    }
}