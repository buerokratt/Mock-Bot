using MockBot.Api.Models;
using MockBot.Api.Services;
using System;
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

            Assert.Equal(chat.Id, result.Id);
        }

        [Fact]
        public void ShouldCreateMessage()
        {
            const string messageContent = "someText";

            var chat = _sut.CreateChat();
            _ = _sut.AddMessage(chat.Id, messageContent);

            var result = _sut.FindById(chat.Id);
            _ = Assert.Single(result.Messages);
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
            var xMessageId = "dmrMessage";
            var xMessageIdRef = message.Id.ToString();
            var xModelType = "good";
            _sut.AddDmrRequest(message);
            var headers = new HeadersInput
            {
                XSentBy = xSentBy,
                XSendTo = xSendTo,
                XMessageId = xMessageId,
                XMessageIdRef = xMessageIdRef,
                XModelType = xModelType
            };

            _sut.AddMessageMetadata(headers);

            Assert.Equal(xSentBy, message.SentBy);
            Assert.Equal(xSendTo, message.SendTo);
            Assert.Equal(xModelType, message.ModelType);
        }

        [Fact]
        public void AddMessageMetadataWithNullShouldThrowNullArgument()
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() => _sut.AddMessageMetadata(null));
            Assert.Equal("Value cannot be null. (Parameter 'headers')", ex.Message);
        }

        [Fact]
        public void AddMessageMetadataMissingMessageRefShouldThrowArgument()
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => _sut.AddMessageMetadata(new HeadersInput() { XMessageIdRef = "Doesn'tExist" }));
            Assert.Equal("Doesn'tExist", ex.Message);
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
            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => _sut.AddMessage(Guid.NewGuid(), "foo"));
            Assert.Equal("Specified argument was out of the range of valid values. (Parameter 'chatId')", ex.Message);
        }
    }
}