using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MockBot.Api.Controllers;
using MockBot.Api.Interfaces;
using Moq;
using RequestProcessor.Models;
using RequestProcessor.Services.Encoder;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MockBot.UnitTests.Controllers
{
    public class DmrControllerTests
    {
        [Fact]
        public async Task ShouldReturnAcceptedAndAddMetadataToMessageAsync()
        {
            // Arrange
            var _mockChatService = new Mock<IChatService>();
            var _mockEncoderService = new Mock<IEncodingService>();
            var _logger = new Mock<ILogger<DmrController>>();

            var message = new Message() { Payload = "An important message" };
            var xSentBy = "sender";
            var xSendTo = "receiver";
            var xMessageId = "dmrMessage";
            var xMessageIdRef = Guid.NewGuid().ToString();
            var xModelType = "good";
            var headers = new HeadersInput
            {
                XSentBy = xSentBy,
                XSendTo = xSendTo,
                XMessageId = xMessageId,
                XMessageIdRef = xMessageIdRef,
                XModelType = xModelType
            };

            var sut = SetupControllerContext(_mockChatService.Object, _mockEncoderService.Object, _logger.Object, "Message");

            // Act
            var result = await sut.PostDmrMessageAsync(headers).ConfigureAwait(false);

            // Assert
            _ = Assert.IsType<AcceptedResult>(result);
            _mockChatService.Verify(cs => cs.AddMessageMetadata(It.IsAny<HeadersInput>()), Times.Once);
        }

        [Fact]
        public async Task DmrCallbackLogsTheCorrectEvent()
        {
            // Arrange
            var _mockChatService = new Mock<IChatService>();
            var _mockEncoderService = new Mock<IEncodingService>();
            var _logger = new Mock<ILogger<DmrController>>();

            var message = new Message() { Payload = "An important message" };
            var xSentBy = "sender";
            var xSendTo = "receiver";
            var xMessageId = "dmrMessage";
            var xMessageIdRef = Guid.NewGuid().ToString();
            var xModelType = "good";
            var headers = new HeadersInput
            {
                XSentBy = xSentBy,
                XSendTo = xSendTo,
                XMessageId = xMessageId,
                XMessageIdRef = xMessageIdRef,
                XModelType = xModelType
            };

            var sut = SetupControllerContext(_mockChatService.Object, _mockEncoderService.Object, _logger.Object, "Message");
            _ = _logger.Setup(l => l.IsEnabled(It.IsAny<LogLevel>())).Returns(true);

            // Act
            _ = await sut.PostDmrMessageAsync(headers).ConfigureAwait(false);

            // Assert
            _logger.Verify(x => x.Log(
               LogLevel.Information,
               new EventId(1, "DmrCallbackReceived"),
               It.Is<It.IsAnyType>((v, t) => true),
               It.IsAny<Exception>(),
               It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)));
        }

        private static DmrController SetupControllerContext(
            IChatService chatService,
            IEncodingService encodingService,
            ILogger<DmrController> logger,
            string input)
        {
            // Create a default HttpContext
            var httpContext = new DefaultHttpContext();

            // Create the stream to house our content
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(input));
            httpContext.Request.Body = stream;
            httpContext.Request.ContentLength = stream.Length;

            return new DmrController(chatService, encodingService, logger)
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