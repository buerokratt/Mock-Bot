using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly Dictionary<Guid, Chat> _chats;

        public ChatControllerTests()
        {
            mockChatService = new Mock<IChatService>();
            sut = new ChatController(mockChatService.Object);
            _chats = new Dictionary<Guid, Chat>();
        }

        [Fact]
        public void FindAll()
        {
            _ = mockChatService.Setup(mock => mock.CreateChat()).Returns(new Chat());
            var chat = sut.Post();
            _chats.Add(chat.Id, chat);

            _ = mockChatService.Setup(mock => mock.FindAll()).Returns(_chats.Values.ToList());
            var findAll = sut.FindAll();

            Assert.Equal(_chats.Count, findAll.Count());
        }

        [Fact]
        public void Get()
        {
            var chat = new Chat();

            _ = mockChatService.Setup(mock => mock.GetId(chat.Id)).Returns(chat);
            var getChat = sut.Get(chat.Id);

            Assert.Equal(chat.Id, getChat.Id);
        }

        [Fact]
        public void PostMessages()
        {
            const string messageContent = "Some text";
            var message = new Message(messageContent);
            var messageList = new List<Message> { message };
            var chat = new Chat();
            chat.Messages.Add(message);

            _ = mockChatService.Setup(mock => mock.AddMessage(chat.Id, messageContent)).Returns(message);
            _ = mockChatService.Setup(mock => mock.GetId(chat.Id)).Returns(chat);

            var postMessage = sut.PostMessage(chat.Id, messageContent);
            var getMessages = sut.GetMessages(chat.Id);

            Assert.Equal(messageContent, postMessage.Content);
            Assert.Equal(messageList.Count, getMessages.Count());
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