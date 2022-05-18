using Microsoft.VisualStudio.TestPlatform.ObjectModel.Utilities;
using MockBot.Api.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MockBot.UnitTests
{
    public class ValuesControllerTests
    {
        private readonly ValuesController sut;

        public ValuesControllerTests()
        {
            sut = new ValuesController();
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

        [Theory]
        [InlineData(1)]
        [InlineData(10000000)]
        [InlineData(-1)]
        public void GetByIdReturnsExpected(int id)
        {
            // Arrange
            var expectedResult ="value";

            // Act
            var result = sut.Get(id);

            // Assert
            Assert.Equal(expectedResult, result);
        }
    }
}
