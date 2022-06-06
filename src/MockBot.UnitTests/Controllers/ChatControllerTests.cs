using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MockBot.Api.Controllers;
using MockBot.Api.Interfaces;
using MockBot.Api.Models;
using MockBot.Api.Services.Dmr;
using Moq;
using Xunit;

namespace MockBot.UnitTests.Controllers
{
    public class ChatControllerTests : ControllerBase
    {
        private readonly ChatController _sut;
        private readonly Mock<IChatService> _mockChatService;
        private readonly Mock<IDmrService> _mockDmrService;

        public ChatControllerTests()
        {
            _mockChatService = new Mock<IChatService>();
            _mockDmrService = new Mock<IDmrService>();
            _sut = new ChatController(_mockChatService.Object, _mockDmrService.Object);
        }

        [Fact]
        public void ShouldReturnSingleChat()
        {
            var chat = new Chat();
            // var createdChat = Assert.IsType<Chat>(chat.Value);
            _ = _mockChatService.Setup(mock => mock.CreateChat())
                .Returns(chat);
            _ = _mockChatService.Setup(mock => mock.FindById(chat.Id)).Returns(chat);

            var result = _sut.Get(chat.Id);

            var resultChat = Assert.IsType<Chat>(result.Value);
            Assert.Equal(chat.Id, resultChat.Id);
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

        // [Fact]
        // public void ShouldAddMessageToChat()
        // {
        //     const string messageContent = "Some text";
        //     var message = new Message(messageContent);
        //     var chat = new Chat();
        //     var currentDateTime = new DateTime();
        //     _ = _mockChatService.Setup(mock => mock.AddMessage(chat.Id, messageContent)).Returns(message);
        //
        //     var result = _sut.PostMessage(chat.Id, messageContent);
        //
        //     var resultMessage = Assert.IsType<Message>(result.Value);
        //     Assert.NotEmpty(resultMessage.Id.ToString());
        //     Assert.True(currentDateTime < resultMessage.CreatedAt);
        // }
    }
}