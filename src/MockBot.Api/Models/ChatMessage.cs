using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace MockBot.Api.Models
{
    public class ChatMessage
    {
        // No logic so no unit tests are required
        [ExcludeFromCodeCoverage]
        public Guid Id { get; } = Guid.NewGuid();

        public string SentBy { get; set; }

        public string SendTo { get; set; }

        public string Content { get; }

        public string Classification { get; set; }

        public DateTime CreatedAt { get; } = DateTime.UtcNow;

        public string ModelType { get; set; }

        public ChatMessage([Required] string content)
        {
            Content = content;
        }
    }
}
