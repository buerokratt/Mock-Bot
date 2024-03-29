﻿using Moq;
using RichardSzalay.MockHttp;
using System;
using System.Net.Http;

namespace MockBot.UnitTests.Extensions
{
    internal static class MockHttpClientFactoryExtensions
    {
        public static Mock<IHttpClientFactory> SetupHttpClientFactory(
            this Mock<IHttpClientFactory> mockHttpClientFactory,
            MockHttpMessageHandler messageHandler)
        {
            _ = mockHttpClientFactory
                .Setup(m => m.CreateClient(It.IsAny<string>()))
                .Returns(() =>
                {
                    var client = messageHandler.ToHttpClient();
                    client.BaseAddress = new Uri("https://tablelogapi.azurewebsites.net/app/fromclassifier");

                    return client;
                });

            return mockHttpClientFactory;
        }
    }
}
