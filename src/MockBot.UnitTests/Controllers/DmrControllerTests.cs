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
using MockBot.Api.Models;
using MockLogging;
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
        private readonly IChatService _chatService = new ChatService();
        private readonly IEncodingService _encodingService = new EncodingService();
        private readonly Mock<IAsyncProcessorService<DmrRequest>> _processor = new();
        private readonly BotSettings _settings = new() { Id = "bot1" };
        private readonly MockLogger<DmrController> _logger = new();

        [Fact]
        public async Task DmrCallbackLogsTheCorrectEvent()
        {
            // Arrange
            var headers = new HeadersInput
            {
                XSentBy = "sender",
                XSendTo = "receiver",
                XMessageId = "dmrMessage",
                XMessageIdRef = Guid.NewGuid().ToString(),
                XModelType = "good"
            };
            var base64DmrRequestPayload = "ewogICAgIkNsYXNzaWZpY2F0aW9uIjoiZWR1Y2F0aW9uIiwKICAgICJNZXNzYWdlIjoiaSB3YW50IHRvIHJlZ2lzdGVyIG15IGNoaWxkIGF0IHNjaG9vbCIKfQ==";

            var sut = SetupControllerContext(
                _chatService,
                _encodingService,
                _processor.Object,
                _settings,
                _logger,
                base64DmrRequestPayload);

            // Act
            _ = await sut.PostDmrMessageAsync(headers).ConfigureAwait(false);

            // Assert
            _ = _logger.VerifyLogEntry()
                .HasLogLevel(LogLevel.Warning);

            _ = _logger.VerifyLogEntry()
                .HasLogLevel(LogLevel.Information)
                .HasEventId(new EventId(1, "DmrCallbackReceived"));
        }

        [Fact]
        public async Task DmrCallbackEnqueuesAcknowledgementWhenModelTypeIsMessageRequest()
        {
            // Arrange
            var headers = new HeadersInput
            {
                XSentBy = "sender",
                XSendTo = "receiver",
                XMessageId = "incomingMessage",
                XMessageIdRef = Guid.NewGuid().ToString(),
                XModelType = ModelTypes.MessageRequest
            };

            var base64DmrRequestPayload = "ewogICAgIkNsYXNzaWZpY2F0aW9uIjoiZWR1Y2F0aW9uIiwKICAgICJNZXNzYWdlIjoiaSB3YW50IHRvIHJlZ2lzdGVyIG15IGNoaWxkIGF0IHNjaG9vbCIKfQ==";

            var sut = SetupControllerContext(
                _chatService,
                _encodingService,
                _processor.Object,
                _settings,
                _logger,
                base64DmrRequestPayload);

            // Act
            _ = await sut.PostDmrMessageAsync(headers).ConfigureAwait(false);

            // Assert
            _processor.Verify(
                m => m.Enqueue(It.Is<DmrRequest>(
                    r => r.Headers.XModelType == Constants.MessageAcknowledgementModelType)));
        }

        [Fact]
        public async Task DmrCallbackReturnsNotFoundWhenArgumentExceptionIsThrown()
        {
            // Arrange
            var base64DmrRequestPayload = "ewogICAgIkNsYXNzaWZpY2F0aW9uIjoiZWR1Y2F0aW9uIiwKICAgICJNZXNzYWdlIjoiaSB3YW50IHRvIHJlZ2lzdGVyIG15IGNoaWxkIGF0IHNjaG9vbCIKfQ==";

            var sut = SetupControllerContext(
                _chatService,
                _encodingService,
                _processor.Object,
                _settings,
                _logger,
                base64DmrRequestPayload);

            // Act
            var response = await sut.PostDmrMessageAsync(null).ConfigureAwait(false);

            // Assert
            _ = Assert.IsType<NotFoundObjectResult>(response);
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