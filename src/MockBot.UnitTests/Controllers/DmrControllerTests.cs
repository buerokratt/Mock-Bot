using Buerokratt.Common.AsyncProcessor;
using Buerokratt.Common.Dmr;
using Buerokratt.Common.Encoder;
using Buerokratt.Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MockBot.Api.Configuration;
using MockBot.Api.Controllers;
using MockBot.Api.Interfaces;
using MockBot.Api.Services;
using Moq;
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
        public async Task DmrCallbackLogsTheCorrectEvent()
        {
            // Arrange
            var chatService = new ChatService();
            var encodingService = new EncodingService();
            var processor = new Mock<IAsyncProcessorService<DmrRequest>>();
            var settings = new BotSettings() { Id = "bot1" };
            var _logger = new Mock<ILogger<DmrController>>();
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
            var base64DmrRequestPayload = "ewogICAgIkNsYXNzaWZpY2F0aW9uIjoiZWR1Y2F0aW9uIiwKICAgICJNZXNzYWdlIjoiaSB3YW50IHRvIHJlZ2lzdGVyIG15IGNoaWxkIGF0IHNjaG9vbCIKfQ==";

            var sut = SetupControllerContext(chatService, encodingService, processor.Object, settings, _logger.Object, base64DmrRequestPayload);
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
            IAsyncProcessorService<DmrRequest> processor,
            BotSettings settings,
            ILogger<DmrController> logger,
            string input)
        {
            // Create a default HttpContext
            var httpContext = new DefaultHttpContext();

            // Create the stream to house our content
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(input));
            httpContext.Request.Body = stream;
            httpContext.Request.ContentLength = stream.Length;

            return new DmrController(chatService, encodingService, processor, settings, logger)
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