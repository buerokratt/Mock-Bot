using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace MockBot.Api.Models
{
    // No logic so no unit tests are required
    [ExcludeFromCodeCoverage]
    public class Chat
    {
        public Guid Id { get; } = Guid.NewGuid();

        [Required]
        public Collection<Message> Messages { get; } = new Collection<Message>();

        public DateTime CreatedAt { get; } = DateTime.UtcNow;
    }

    // No logic so no unit tests are required
    [ExcludeFromCodeCoverage]
    public class Message
    {
        public Guid Id { get; } = Guid.NewGuid();

        public string? SentBy { get; set; }

        public string? SendTo { get; set; }

        public string Content { get; }

        public DateTime CreatedAt { get; } = DateTime.UtcNow;

        public string? ModelType { get; set; }

        public Message([Required] string content)
        {
            Content = content;
        }
    }
}