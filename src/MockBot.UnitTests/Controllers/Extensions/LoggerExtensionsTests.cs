using Microsoft.Extensions.Logging;
using MockBot.Api.Controllers.Extensions;
using Moq;
using System;
using Xunit;

namespace MockBot.UnitTests.Controllers.Extensions
{
    public class LoggerExtensionsTests
    {
        private readonly Mock<ILogger> _mockLogger;

        public LoggerExtensionsTests()
        {
            _mockLogger = new Mock<ILogger>();
            _ = _mockLogger
                .Setup(m => m.IsEnabled(It.IsAny<LogLevel>()))
                .Returns(true);
        }

        [Fact]
        public void DmrCallbackShouldLogTelemetry()
        {
            var logger = _mockLogger.Object;

            logger.DmrCallbackReceived("bot1", "message1", "encoded", "decoded");

            _mockLogger.Verify(
                m => m.Log(
                    LogLevel.Information,
                    It.Is<EventId>(e => e.Id == 1 && e.Name == "DmrCallbackReceived"),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)));
        }
    }
}
