using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MockBot.Api.Configuration;
using MockBot.Api.Controllers;
using MockBot.Api.Interfaces;
using MockBot.Api.Models;
using MockBot.Api.Services;
using Moq;
using RequestProcessor.AsyncProcessor;
using RequestProcessor.Dmr;
using RequestProcessor.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MockBot.UnitTests.Controllers
{
    public class ChatControllerTests : ControllerBase
    {
        private readonly ChatController _sut;
        private readonly Mock<IChatService> _mockChatService;
        private readonly IChatService _chatService;
        private readonly Mock<IAsyncProcessorService<DmrRequest>> _mockDmrService;
        private readonly HeadersInput _headers;

        public ChatControllerTests()
        {
            _mockChatService = new Mock<IChatService>();
            _chatService = new ChatService();
            _mockDmrService = new Mock<IAsyncProcessorService<DmrRequest>>();
            _sut = new ChatController(_chatService, _mockDmrService.Object, new BotSettings() { Id = "unitTestBot" });
            _headers = new HeadersInput
            {
                XSentBy = "unitTestBot",
                XSendTo = "receiver",
                XModelType = "somemodel"
            };
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
            // Arrange
            _sut.ControllerContext = new ControllerContext()
            {
                HttpContext = GetContext(payload)
            };

            var chat = _chatService.CreateChat();
            var currentDateTime = new DateTime();

            // Act
            var result = await _sut.PostMessageAsync(chat.Id).ConfigureAwait(false);

            // Assert
            var createdResult = Assert.IsType<CreatedResult>(result);
            var resultMessage = Assert.IsType<ChatMessage>(createdResult.Value);
            Assert.Equal($"/chats/{chat.Id}/messages", createdResult.Location);
            Assert.NotEmpty(resultMessage.Id.ToString());
            Assert.True(currentDateTime < resultMessage.CreatedAt);
        }

        [Theory]
        [InlineData("Some text")]
        public async Task ShouldFailToAddMessageToChatWhenNoChatWithGivenIdAsync(string payload)
        {
            // Arrange
            _sut.ControllerContext = new ControllerContext()
            {
                HttpContext = GetContext(payload)
            };
            var chatId = Guid.NewGuid();

            // Act
            var result = await _sut.PostMessageAsync(chatId).ConfigureAwait(false);

            // Assert
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