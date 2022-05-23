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
    }

    public class Message
    {
        public Guid Id { get; } = Guid.NewGuid();

        public string Content { get; set; }

        public DateTime CreatedAt { get; } = DateTime.Now;

        public Message([Required] string content)
        {
            Content = content;
        }
    }
}