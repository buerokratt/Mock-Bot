using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace MockBot.Api.Models
{
    // No logic so no unit tests are required
    [ExcludeFromCodeCoverage]
    public class HeadersInput
    {
        [Required]
        [FromHeader(Name = "X-Sent-By")]
        public string? XSentBy { get; set; }

        [Required]
        [FromHeader(Name = "X-Send-To")]
        public string? XSendTo { get; set; }

        [Required]
        [FromHeader(Name = "X-Message-Id")]
        public string? XMessageId { get; set; }

        [FromHeader(Name = "X-Message-Id-Ref")]
        public string? XMessageIdRef { get; set; }

        [FromHeader(Name = "X-Model-Type")]
        public string? XModelType { get; set; }
    }
}