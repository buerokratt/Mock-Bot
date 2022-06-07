using Microsoft.AspNetCore.Mvc;
using MockBot.Api.Controllers;
using MockBot.Api.Interfaces;
using MockBot.Api.Models;
using Moq;
using Xunit;

namespace MockBot.UnitTests.Controllers
{
    public class DmrControllerTests
    {
        private readonly DmrController _sut;
        private readonly Mock<IChatService> _mockChatService;

        public DmrControllerTests()
        {
            _mockChatService = new Mock<IChatService>();
            _sut = new DmrController(_mockChatService.Object);
        }

        [Fact]
        public void ShouldReturnAcceptedAndAddMetadataToMessage()
        {
            var message = new Message("An important message");
            var xSentBy = "sender";
            var xSendTo = "receiver";
            var xMessageId = "dmrMessage";
            var xMessageIdRef = message.Id.ToString();
            var xModelType = "good";
            var headers = new HeadersInput
            {
                XSentBy = xSentBy,
                XSendTo = xSendTo,
                XMessageId = xMessageId,
                XMessageIdRef = xMessageIdRef,
                XModelType = xModelType
            };

            var result = _sut.PostDmrMessageAsync(headers);

            _ = Assert.IsType<AcceptedResult>(result);
        }
    }
}