using MockBot.Api.Models;
using MockBot.Api.Services;
using Xunit;

namespace MockBot.UnitTests.Services
{
    public class DmrServiceTests
    {
        private readonly DmrService _sut;

        public DmrServiceTests()
        {
            _sut = new DmrService();
        }

        [Fact]
        public void ShouldAddMessageToDictionary()
        {
            var message = new Message("Hello world");

            _sut.AddDmrRequest(message);

            Assert.Equal(1, _sut.DmrRequests.Count);
        }
    }
}