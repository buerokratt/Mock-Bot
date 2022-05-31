using MockBot.Api.Models;
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

        [Fact]
        public void ShouldAddDmrRequests()
        {
            var message = new Message("Hello");

            _sut.AddDmrRequest(message);

            Assert.Equal(1, _sut.DmrRequests.Count);
        }

        [Fact]
        public void ShouldAddMessageMetadata()
        {
            var message = new Message("Hello");
            var xSentBy = "sender";
            var xSendTo = "receiver";
            var xMessageId = "some id";
            var xModelType = "good";
            _sut.AddDmrRequest(message);

            _sut.AddMessageMetadata(xSentBy, xSendTo, xMessageId, message.Id.ToString(), xModelType);

            Assert.Equal(xSentBy, message.SentBy);
            Assert.Equal(xSendTo, message.SendTo);
            Assert.Equal(xModelType, message.ModelType);
        }
    }
}