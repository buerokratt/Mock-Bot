using System.Collections.Generic;
using MockBot.Api.Models;
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
        var message = new Message("someText");
        var chat = new Chat
        {
            Messages = new List<Message> { message }
        };

        var result = _sut.Create();
        result.Messages.Add(message);

        Assert.Equal(chat.Messages, result.Messages);
        Assert.NotEmpty(result.Id.ToString());
    }
}