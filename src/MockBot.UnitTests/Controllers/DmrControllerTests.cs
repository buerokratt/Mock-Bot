using System.IO;
using System.Text;
using Microsoft.AspNetCore.Http;
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

        [Theory]
        [InlineData("Some text")]
        public void ShouldReturnAcceptedAndAddMetadataToMessage(string payload)
        {
            _sut.ControllerContext = new ControllerContext()
            {
                HttpContext = GetContext(payload),
            };

            var result = _sut.PostDmrMessage();

            Assert.Equal(202, result.StatusCode);
        }

        private static DefaultHttpContext GetContext(string payload)
        {
            var httpContext = new DefaultHttpContext();
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(payload));
            httpContext.Request.Headers[Constants.SendToHeaderKey] = "Classifier";
            httpContext.Request.Body = stream;
            httpContext.Request.ContentLength = stream.Length;
            return httpContext;
        }
    }
}