using MockBot.Api.Controllers;
using Xunit;

namespace MockBot.UnitTests
{
    public class Values2ControllerTests
    {
        private readonly Values2Controller sut;

        public Values2ControllerTests()
        {
            sut = new Values2Controller();
        }

        [Fact]
        public void GetReturnsExpected()
        {
            // Arrange
            var expectedResult = new string[] { "value1", "value2" };

            // Act
            var result = sut.Get();

            // Assert
            Assert.Equal(expectedResult, result);
        }
    }
}
