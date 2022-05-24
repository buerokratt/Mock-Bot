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
            var findAllChats = sut.FindAll();

            Assert.Equal(_chats.Count, findAllChats.Count());
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