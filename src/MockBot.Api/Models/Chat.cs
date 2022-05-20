using System.ComponentModel.DataAnnotations;

namespace MockBot.Api.Models;

public class Chat
{
    public Guid Id { get; } = Guid.NewGuid();

    [Required] public List<Message> Messages { get; } = new List<Message>();
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