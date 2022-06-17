using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MockBot.Api.Configuration;
using MockBot.Api.Controllers;
using MockBot.Api.Interfaces;
using MockBot.Api.Models;
using Moq;
using RequestProcessor.AsyncProcessor;
using RequestProcessor.Dmr;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Chat = MockBot.Api.Models.Chat;

namespace MockBot.UnitTests.Controllers
{
    public class ChatControllerTests : ControllerBase
    {
        private readonly ChatController _sut;
        private readonly Mock<IChatService> _mockChatService;
        private readonly Mock<IAsyncProcessorService<DmrRequest>> _mockDmrService;

        public ChatControllerTests()
        {
            _mockChatService = new Mock<IChatService>();
            _mockDmrService = new Mock<IAsyncProcessorService<DmrRequest>>();
            _sut = new ChatController(_mockChatService.Object, _mockDmrService.Object, new BotSettings() { Id = "bot1" });
        }

        [Fact]
        public void ShouldReturnSingleChat()
        {
            // Arrange
            var chat = new Chat();
            _ = _mockChatService.Setup(mock => mock.CreateChat())
                .Returns(chat);
            _ = _mockChatService.Setup(mock => mock.FindById(chat.Id)).Returns(chat);

            // Act
            var response = _sut.Get(chat.Id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(response.Result);
            var resultChat = Assert.IsType<Chat>(okResult.Value);
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

        [Theory]
        [InlineData("Some text")]
        public async Task ShouldAddMessageToChatAsync(string payload)
        {
            _sut.ControllerContext = new ControllerContext()
            {
                HttpContext = GetContext(payload)
            };

            var message = new ChatMessage(payload);
            var chat = new Chat();
            var currentDateTime = new DateTime();
            _ = _mockChatService.Setup(mock => mock.AddMessage(chat.Id, payload)).Returns(message);

            var result = await _sut.PostMessageAsync(chat.Id).ConfigureAwait(false);

            var createdResult = Assert.IsType<CreatedResult>(result);
            var resultMessage = Assert.IsType<ChatMessage>(createdResult.Value);
            Assert.Equal($"/chats/{chat.Id}/messages", createdResult.Location);
            Assert.NotEmpty(resultMessage.Id.ToString());
            Assert.True(currentDateTime < resultMessage.CreatedAt);
            _mockChatService.Verify(mock => mock.AddDmrRequest(message));
            _mockDmrService.Verify(mock => mock.Enqueue(It.IsAny<DmrRequest>()));
        }

        [Theory]
        [InlineData("Some text")]
        public async Task ShouldFailToAddMessageToChatWhenNoChatWithGivenIdAsync(string payload)
        {
            _sut.ControllerContext = new ControllerContext()
            {
                HttpContext = GetContext(payload)
            };
            var chat = new Chat();
            _ = _mockChatService.Setup(mock => mock.AddMessage(chat.Id, payload)).Throws(new ArgumentOutOfRangeException(chat.Id.ToString()));


            var result = await _sut.PostMessageAsync(chat.Id).ConfigureAwait(false);

            _ = Assert.IsType<NotFoundObjectResult>(result);
        }

        private static DefaultHttpContext GetContext(string payload)
        {
            var httpContext = new DefaultHttpContext();
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(payload));
            httpContext.Request.Body = stream;
            httpContext.Request.ContentLength = stream.Length;
            return httpContext;
        }
    }
}