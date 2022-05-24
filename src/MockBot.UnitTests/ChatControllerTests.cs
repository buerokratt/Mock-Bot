using System;
using System.Collections.Generic;
using MockBot.Api.Controllers;
using MockBot.Api.Interfaces;
using MockBot.Api.Models;
using Moq;
using Xunit;

namespace MockBot.UnitTests
{
    public class ChatControllerTests
    {
        private readonly ChatController _sut;
        private readonly Mock<IChatService> _mockChatService;

        public ChatControllerTests()
        {
            _mockChatService = new Mock<IChatService>();
            _sut = new ChatController(_mockChatService.Object);
        }

        [Fact]
        public void ShouldReturnAllChats()
        {
            var chat1 = new Chat();
            var chat2 = new Chat();
            var mockChats = new List<Chat>() { chat1, chat2 };
            _ = _mockChatService.Setup(mock => mock.FindAll()).Returns(mockChats);

            var result = _sut.FindAll();

            Assert.Equal(200, result.StatusCode);
            var resultList = Assert.IsType<List<Chat>>(result.Value);
            Assert.Equal(2, resultList.Count);
            Assert.Contains(chat1, resultList);
            Assert.Contains(chat2, resultList);
        }

        [Fact]
        public void ShouldCreateAndReturnChat()
        {
            var chat = new Chat();
            _ = _mockChatService.Setup(mock => mock.CreateChat()).Returns(chat);

            var result = _sut.Post();

            Assert.Equal(201, result.StatusCode);
            var resultChat = Assert.IsType<Chat>(result.Value);
            Assert.NotEmpty(resultChat.Id.ToString());
            Assert.Equal($"/chats/{resultChat.Id}", result.Location);
        }

        [Fact]
        public void ShouldAddMessageToChat()
        {
            const string messageContent = "Some text";
            var message = new Message(messageContent);
            var chat = new Chat();
            var currentDateTime = new DateTime();
            _ = _mockChatService.Setup(mock => mock.AddMessage(chat.Id, messageContent)).Returns(message);

            var result = _sut.PostMessage(chat.Id, messageContent);

            var resultMessage = Assert.IsType<Message>(result.Value);
            Assert.NotEmpty(resultMessage.Id.ToString());
            Assert.True(currentDateTime < resultMessage.CreatedAt);
        }
    }
}