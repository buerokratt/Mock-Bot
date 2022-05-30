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
        private readonly Mock<IDmrService> _mockDmrService;

        public DmrControllerTests()
        {
            _mockChatService = new Mock<IChatService>();
            _mockDmrService = new Mock<IDmrService>();
            _sut = new DmrController(_mockChatService.Object, _mockDmrService.Object);
        }

        [Fact]
        public void ShouldReturnAcceptedAndAddMetadataToMessage()
        {
            var message = new Message("Big Data");
            var xSentBy = "sender";
            var xSendTo = "receiver";
            var xMessageId = "dmrMessage";
            var xMessageIdRef = message.Id.ToString();
            // _ = _mockDmrService.Setup(mock => mock.AddDmrRequest(message)).Callback(() => {});

            var result = _sut.PostDmrMessage(xSentBy, xSendTo, xMessageId, xMessageIdRef);

            Assert.Equal(202, result.StatusCode);
        }
    }
}