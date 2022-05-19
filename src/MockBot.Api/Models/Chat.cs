using System.ComponentModel.DataAnnotations;

namespace MockBot.Api.Models;

public class Chat
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required] public List<Message> Messages { get; set; } = new List<Message>();
}

public class Message
{
    private Guid Id { get; set; } = Guid.NewGuid();
 
    private string Content { get; set; }
    
    private DateTime CreatedAt { get; set; } = DateTime.Now;
    
    public Message([Required] string content)
    {
        Content = content;
    }

}