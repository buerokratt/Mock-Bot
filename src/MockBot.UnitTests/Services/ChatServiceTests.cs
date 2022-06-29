using MockBot.Api.Models;
using MockBot.Api.Services;
using RequestProcessor.Models;
using System;
using Xunit;

namespace MockBot.UnitTests.Services
{
    public class ChatServiceTests
    {
        private readonly ChatService _sut;
        private readonly HeadersInput _headers;

        public ChatServiceTests()
        {
            _sut = new ChatService();
            _headers = new HeadersInput
            {
                XSentBy = "sender",
                XSendTo = "receiver",
                XMessageId = "UnitTestMessage",
                XMessageIdRef = "",
                XModelType = "somemodel"
            };
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

            Assert.Equal(chat.Id, result.Id);
        }

        [Fact]
        public void ShouldCreateMessage()
        {
            const string messageContent = "someText";

            var chat = _sut.CreateChat();
            _ = _sut.AddMessage(chat.Id, messageContent, _headers);

            var result = _sut.FindById(chat.Id);
            _ = Assert.Single(result.Messages);
        }

        [Fact]
        public void ShouldAddDmrRequests()
        {
            var message = new ChatMessage("Hello");

            _sut.AddDmrRequest(message);

            Assert.Equal(1, _sut.DmrRequests.Count);
        }

        [Fact]
        public void AddDmrRequestWithNullShouldThrowNullArgument()
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() => _sut.AddDmrRequest(null));
            Assert.Equal("Value cannot be null. (Parameter 'message')", ex.Message);
        }

        [Fact]
        public void AddMessageInvalidChatIdShouldThrowArgumentOutOfRangeException()
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => _sut.AddMessage(Guid.NewGuid(), "foo", _headers));
            Assert.Equal("Specified argument was out of the range of valid values. (Parameter 'chatId')", ex.Message);
        }
    }
}