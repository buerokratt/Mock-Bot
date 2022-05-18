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

            // Act
            var result = sut.Get();

            // Assert
            Assert.Equal(new string[] { "value1", "value2" }, result);
    }
    }
}
