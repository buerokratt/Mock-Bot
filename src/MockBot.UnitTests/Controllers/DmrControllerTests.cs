using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MockBot.Api.Controllers;
using MockBot.Api.Interfaces;
using MockBot.Api.Models;
using Moq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MockBot.UnitTests.Controllers
{
    public class DmrControllerTests
    {
        private readonly Mock<IChatService> _mockChatService;
        private readonly Mock<IEncodingService> _mockEncoderService;

        public DmrControllerTests()
        {
            _mockChatService = new Mock<IChatService>();
            _mockEncoderService = new Mock<IEncodingService>();
        }

        [Fact]
        public async Task ShouldReturnAcceptedAndAddMetadataToMessageAsync()
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

            var sut = SetupControllerContext(_mockChatService.Object, _mockEncoderService.Object, "Message");

            var result = await sut.PostDmrMessageAsync(headers).ConfigureAwait(false);

            _ = Assert.IsType<AcceptedResult>(result);
        }

        private static DmrController SetupControllerContext(IChatService chatService, IEncodingService encodingService, string input)
        {
            // Create a default HttpContext
            var httpContext = new DefaultHttpContext();

            // Create the stream to house our content
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(input));
            httpContext.Request.Body = stream;
            httpContext.Request.ContentLength = stream.Length;

            return new DmrController(chatService, encodingService, new Mock<ILogger<DmrController>>().Object)
            {
                // Set the controller context to our created HttpContext
                ControllerContext = new ControllerContext()
                {
                    HttpContext = httpContext
                }
            };
        }
    }
}