using MockBot.Api.Services;
using Xunit;

namespace MockBot.UnitTests.Services
{
    public class ChatServiceTests
    {
        private readonly ChatService _sut;

        public ChatServiceTests()
        {
            _sut = new ChatService();
        }

        [Fact]
        public void ShouldCreateChat()
        {
            var result = _sut.CreateChat();

            Assert.NotEmpty(result.Id.ToString());
        }

        [Fact]
        public void ShouldFindAllCreatedChats()
        {
            var chat = _sut.CreateChat();
            var result = _sut.FindAll();

            Assert.Contains(chat, result);
        }

        [Fact]
        public void ShouldGetChatById()
        {
            var chat = _sut.CreateChat();
            var result = _sut.FindById(chat.Id);

            Assert.Equal(chat.Id, result!.Id);
        }

        [Fact]
        public void ShouldCreateMessage()
        {
            const string messageContent = "someText";

            var chat = _sut.CreateChat();
            _ = _sut.AddMessage(chat.Id, messageContent);

            var result = _sut.FindById(chat.Id);
            Assert.NotNull(result!.Messages);
        }
    }
}