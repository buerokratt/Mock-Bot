using MockBot.Api.Services;
using Xunit;

namespace MockBot.UnitTests
{
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
            var result = _sut.CreateChat();

            Assert.NotEmpty(result.Id.ToString());
        }

        [Fact]
        public void FindAll()
        {
            var chat = _sut.CreateChat();
            var result = _sut.FindAll();

            Assert.Contains(chat, result);
        }

        [Fact]
        public void GetId()
        {
            var chat = _sut.CreateChat();
            var result = _sut.FindById(chat.Id);

            Assert.Equal(chat.Id, result.Id);
        }

        [Fact]
        public void CreateMessage()
        {
            const string messageContent = "someText";

            var chat = _sut.CreateChat();
            _ = _sut.AddMessage(chat.Id, messageContent);

            Assert.NotNull(_sut.FindById(chat.Id).Messages);
        }
    }
}