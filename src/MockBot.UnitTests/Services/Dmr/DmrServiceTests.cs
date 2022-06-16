using Microsoft.Extensions.Logging;
using MockBot.Api.Services.Dmr;
using MockBot.UnitTests.Extensions;
using Moq;
using RequestProcessor.Models;
using RichardSzalay.MockHttp;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace MockBot.UnitTests.Services.Dmr
{
    public sealed class DmrServiceTests : IDisposable
    {
        private static readonly DmrServiceSettings DefaultServiceConfig = new()
        {
            DmrApiUri = new Uri("https://dmr.fakeurl.com")
        };

        private readonly MockHttpMessageHandler httpMessageHandler = new();
        private readonly Mock<ILogger<DmrService>> logger = new();

        [Fact]
        public async Task ShouldCallDmrApiWithGivenRequestWhenRequestIsRecorded()
        {
            _ = httpMessageHandler.SetupWithExpectedMessage();

            var clientFactory = GetHttpClientFactory(httpMessageHandler, DefaultServiceConfig);

            var sut = new DmrService(clientFactory.Object, DefaultServiceConfig, logger.Object);

            sut.Enqueue(GetDmrRequest());

            await sut.ProcessRequestsAsync().ConfigureAwait(false);

            httpMessageHandler.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task ShouldCallDmrApiForEachGivenRequestWhenMultipleRequestsAreRecorded()
        {
            _ = httpMessageHandler
                .SetupWithExpectedMessage("my first message", "education")
                .SetupWithExpectedMessage("my second message", "social");

            var clientFactory = GetHttpClientFactory(httpMessageHandler, DefaultServiceConfig);

            var sut = new DmrService(clientFactory.Object, DefaultServiceConfig, logger.Object);

            sut.Enqueue(GetDmrRequest("my first message", "education"));
            sut.Enqueue(GetDmrRequest("my second message", "social"));

            await sut.ProcessRequestsAsync().ConfigureAwait(false);

            httpMessageHandler.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task ShouldNotThrowExceptionWhenCallToDmrApiErrors()
        {
            using var dmrHttpClient = new MockHttpMessageHandler();
            _ = dmrHttpClient.When("/").Respond(HttpStatusCode.BadGateway);

            var clientFactory = GetHttpClientFactory(dmrHttpClient, DefaultServiceConfig);

            var sut = new DmrService(clientFactory.Object, DefaultServiceConfig, logger.Object);

            sut.Enqueue(GetDmrRequest());

            await sut.ProcessRequestsAsync().ConfigureAwait(false);
        }

        private static Mock<IHttpClientFactory> GetHttpClientFactory(MockHttpMessageHandler messageHandler, DmrServiceSettings settings)
        {
            settings ??= DefaultServiceConfig;

            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            _ = mockHttpClientFactory
                .Setup(m => m.CreateClient(It.IsAny<string>()))
                .Returns(() =>
                {
                    var client = messageHandler.ToHttpClient();
                    client.BaseAddress = settings.DmrApiUri;

                    return client;
                });

            return mockHttpClientFactory;
        }


        private static DmrRequest GetDmrRequest(string message = "my test message", string classification = "border")
        {
            var headers = new HeadersInput
            {
                XSentBy = "MockClassifier.UnitTests.Services.Dmr.DmrServiceTests",
                XSendTo = "Classifier",
                XMessageId = "1f7b356d-a6f4-4aeb-85cd-9d570dbc7606",
                XMessageIdRef = "5822c6ef-177d-4dd7-b4c5-0d9d8c8d2c35",
                XModelType = "application/vnd.classifier.classification+json;version=1",
                ContentType = "text/plain"
            };

            var request = new DmrRequest(headers)
            {
                Payload = new DmrRequestPayload
                {
                    Message = message,
                    Classification = classification
                }
            };

            return request;
        }

        public void Dispose()
        {
            httpMessageHandler.Dispose();
        }
    }
}
