﻿using MockBot.Api.Models;
using MockBot.Api.Services.Dmr.Extensions;
using System.Collections.Concurrent;
using System.Net.Mime;
using System.Text;
using System.Text.Json;

namespace MockBot.Api.Services.Dmr
{
    /// <summary>
    /// A service that handles calls to the DMR API
    /// </summary>
    public class DmrService : IDmrService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<DmrService> _logger;

        private readonly ConcurrentQueue<DmrRequest> requests;

        public DmrService(IHttpClientFactory httpClientFactory, DmrServiceSettings config, ILogger<DmrService> logger)
        {
            _ = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _ = config ?? throw new ArgumentNullException(nameof(config));

            _httpClient = httpClientFactory.CreateClient(config.ClientName);
            _httpClient.BaseAddress = config.DmrApiUri;
            _logger = logger;

            requests = new ConcurrentQueue<DmrRequest>();
        }

        public void RecordRequest(DmrRequest request)
        {
            requests.Enqueue(request);
        }

        public async Task ProcessRequestsAsync()
        {
            while (requests.TryDequeue(out var request))
            {
                try
                {
                    // Setup content
                    var jsonPayload = JsonSerializer.Serialize(request.Payload);
                    var jsonPayloadBase64 = EncodeBase64(jsonPayload);
                    using var content = new StringContent(jsonPayloadBase64, Encoding.UTF8,
                        MediaTypeNames.Application.Json);

                    // Setup message
                    using var requestMessage = CreateRequestMessage(request, content);

                    // Send request
                    var response = await _httpClient.SendAsync(requestMessage).ConfigureAwait(false);
                    _ = response.EnsureSuccessStatusCode();

                    _logger.DmrCallback(request.Payload!.Classification, request.Payload.Message);
                }
                catch (HttpRequestException exception)
                {
                    _logger.DmrCallbackFailed(exception);
                }
            }
        }

        private static string EncodeBase64(string content)
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            ;

            byte[] bytes = Encoding.UTF8.GetBytes(content);
            var base64 = Convert.ToBase64String(bytes);
            return base64;
        }

        private static HttpRequestMessage CreateRequestMessage(DmrRequest request, StringContent content)
        {
            var requestMessage = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                Content = content,
            };

            requestMessage.Headers.Add(Constants.MessageIdHeaderKey, request.Headers[Constants.MessageIdHeaderKey]);
            requestMessage.Headers.Add(Constants.MessageIdRefHeaderKey,
                request.Headers[Constants.MessageIdRefHeaderKey]);
            requestMessage.Headers.Add(Constants.SendToHeaderKey, request.Headers[Constants.SendToHeaderKey]);
            requestMessage.Headers.Add(Constants.SentByHeaderKey, request.Headers[Constants.SentByHeaderKey]);
            requestMessage.Headers.Add(Constants.ModelTypeHeaderKey, request.Headers[Constants.ModelTypeHeaderKey]);

            return requestMessage;
        }
    }
}