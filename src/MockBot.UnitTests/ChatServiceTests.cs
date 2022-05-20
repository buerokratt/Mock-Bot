using MockBot.Api.Services;
using Xunit;

namespace MockBot.UnitTests;

public class ChatServiceTests
{
    private readonly ChatService _sut;

    public ChatServiceTests()
    {
        _sut = new ChatService();
    }

    [Fact]
    public void Create()
    {
        var result = _sut.Create();

        Assert.NotEmpty(result.Id.ToString());
    }

    [Fact]
    public void FindAll()
    {
        _sut.Create();
        var result = _sut.FindAll();
        
        Assert.Single(result);
    }

    [Fact]
    public void Get()
    {
        var chat = _sut.Create();
        var result = _sut.Get(chat.Id);
        
        Assert.Equal(chat.Id, result.Id);
    }

    [Fact]
    public void CreateMessage()
    {
        var messageContent = "someText";

        var chat = _sut.Create();
        var message = _sut.CreateMessage(chat.Id, messageContent);
        var result = _sut.Get(chat.Id).Messages.Find(m => m.Id.Equals(message.Id)).Content;
        
        Assert.Equal(messageContent, result);
        Assert.NotNull(message.CreatedAt);
    }
}