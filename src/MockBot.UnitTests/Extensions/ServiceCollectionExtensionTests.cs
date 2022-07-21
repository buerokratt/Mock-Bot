using AspNetCore.Authentication.ApiKey;
using Buerokratt.Common.CentOps;
using Buerokratt.Common.CentOps.Interfaces;
using Buerokratt.Common.Dmr;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MockBot.Api;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace MockBot.UnitTests.Extensions
{
    public class ServiceCollectionExtensionTests
    {
        [Fact]
        public void AddDmrCommunicationsThrowsForMissingConfiguration()
        {
            // Arrange
            IServiceCollection services = new ServiceCollection();

            // Act & Assert
            _ = Assert.Throws<ArgumentNullException>(() => services.AddDmrCommunications(null));
        }

        [Fact]
        public void AddDmrCommunicationsAddsCorrectTypes()
        {
            // Arrange
            IServiceCollection services = new ServiceCollection();

            var inMemorySettings = new Dictionary<string, string> {
                {"DmrServiceSettings:CentOpsApiKey", "value"},
                {"DmrServiceSettings:CentOpsUrl", "value"},
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            // Act
            services.AddDmrCommunications(configuration);

            // Assert
            // Ensure that there's an ICentOpsService Implementation otherwise code needs to change.
            var interfaces = services.Select(s => s.ServiceType).ToArray();
            _ = interfaces.Should().Contain(typeof(ICentOpsService));
        }

        [Fact]
        public void AddApiAuthenticationThrowsForMissingConfiguration()
        {
            // Arrange
            IServiceCollection services = new ServiceCollection();

            // Act & Assert
            _ = Assert.Throws<ArgumentNullException>(() => services.AddApiAuthentication(null));
        }

        [Fact]
        public void AddApiAuthenticationForMissingApiKeyConfig()
        {
            // Arrange
            IServiceCollection services = new ServiceCollection();

            var inMemorySettings = new Dictionary<string, string> {
                {"DmrServiceSettings:CentOpsApiKey", "value"},
                {"DmrServiceSettings:CentOpsUrl", "value"},
                {"ConnectionStrings:ApiKey", string.Empty},
            };

            IConfiguration configuration = new ConfigurationBuilder()
               .AddInMemoryCollection(inMemorySettings)
               .Build();

            // Act & Assert
            _ = Assert.Throws<ArgumentException>(() => services.AddApiAuthentication(configuration));
        }


        [Fact]
        public void AddApiAuthenticationAddsCorrectTypes()
        {
            // Arrange
            IServiceCollection services = new ServiceCollection();

            var inMemorySettings = new Dictionary<string, string> {
                {"DmrServiceSettings:CentOpsApiKey", "value"},
                {"DmrServiceSettings:CentOpsUrl", "value"},
                {"ConnectionStrings:ApiKey", "key"},
            };

            IConfiguration configuration = new ConfigurationBuilder()
               .AddInMemoryCollection(inMemorySettings)
               .Build();

            // Act
            services.AddApiAuthentication(configuration);

            // Assert
            // Ensure that there's an ICentOpsService Implementation otherwise code needs to change.
            var interfaces = services.Select(s => s.ServiceType).ToArray();
            _ = interfaces.Should().Contain(typeof(ApiKeyInHeaderHandler));
        }
    }
}
