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
        public Collection<ChatMessage> Messages { get; } = new Collection<ChatMessage>();

        public DateTime CreatedAt { get; } = DateTime.UtcNow;
    }
}