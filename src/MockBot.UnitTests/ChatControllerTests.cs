using MockBot.Api.Controllers;
using MockBot.Api.Interfaces;
using MockBot.Api.Models;
using Moq;
using Xunit;

namespace MockBot.UnitTests
{
    public class ChatControllerTests
    {
        private readonly ChatController sut;
        private readonly Mock<IChatService> mockChatService;

        public ChatControllerTests()
        {
            mockChatService = new Mock<IChatService>();
            sut = new ChatController(mockChatService.Object);
        }

        [Fact]
        public void Post()
        {
            _ = mockChatService.Setup(mock => mock.CreateChat()).Returns(new Chat());
            var chat = sut.Post();

            Assert.NotNull(chat);
        }
    }
}