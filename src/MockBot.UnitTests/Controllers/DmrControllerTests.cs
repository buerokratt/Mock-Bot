using Microsoft.AspNetCore.Http;
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

        // [Fact]
        // public void ShouldReturnAcceptedAndAddMetadataToMessage()
        // {
        //     // var message = new Message("Big Data");
        //     // var xSentBy = "sender";
        //     // var xSendTo = "receiver";
        //     // var xMessageId = "dmrMessage";
        //     // var xModelType = "good";
        //     // var xMessageIdRef = message.Id.ToString();
        //
        //     // var headers = new HeaderDictionary
        //     // {
        //     //     { Constants.MessageIdRefHeaderKey, xMessageIdRef},
        //     //     { Constants.SendToHeaderKey, xSendTo },
        //     //     { Constants.SentByHeaderKey, xSentBy },
        //     //     { Constants.ModelTypeHeaderKey, xModelType }
        //     // };
        //
        //     var result = _sut.PostDmrMessage();
        //
        //     Assert.Equal(202, result.StatusCode);
        // }
    }
}