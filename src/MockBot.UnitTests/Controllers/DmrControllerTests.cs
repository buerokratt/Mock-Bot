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
            var message = new Message("Big Data");
            var xSentBy = "sender";
            var xSendTo = "receiver";
            var xMessageId = "dmrMessage";
            var xModelType = "good";
            var xMessageIdRef = message.Id.ToString();

            var result = _sut.PostDmrMessage(new HeadersInput
            {
                XMessageId = xMessageId,
                XMessageIdRef = xMessageIdRef,
                XSendTo = xSendTo,
                XSentBy = xSentBy,
                XModelType = xModelType
            });

            Assert.Equal(202, result.StatusCode);
        }
    }
}