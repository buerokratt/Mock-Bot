using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MockBot.Api.Services.Dmr;
using MockBot.Api.Services.Dmr.Extensions;
using RequestProcessor.AsyncProcessor;
using System;
using System.Linq;
using Xunit;

namespace MockBot.UnitTests.Services.Dmr.Extensions
{
    public sealed class ServiceCollectionExtensionsTests
    {
        [Fact]
        public void AddDmrServiceAddsServices()
        {
            //Arrange
            DmrServiceSettings DefaultServiceConfig = new()
            {
                DmrApiUri = new Uri("https://dmr.fakeurl.com"),
                ClientName = "Foo",
            };
            var collection = new ServiceCollection();

            // Act
            collection.AddDmrService(DefaultServiceConfig);
            var dmrService = collection.First(e => e.ServiceType == typeof(IAsyncProcessorService<DmrRequest>));
            var dmrServiceSettings = collection.First(e => e.ServiceType == typeof(DmrServiceSettings));
            var dmrHostedService = collection.First(e => e.ServiceType == typeof(IHostedService));

            // Assert
            Assert.Equal(ServiceLifetime.Singleton, dmrService.Lifetime);
            Assert.Equal(ServiceLifetime.Singleton, dmrServiceSettings.Lifetime);
            Assert.Equal(ServiceLifetime.Singleton, dmrHostedService.Lifetime);
        }
    }
}
