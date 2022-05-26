using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using MockBot.Api.Interfaces;
using MockBot.Api.Models;

namespace MockBot.Api.Services
{
    public class DmrService : IDmrService
    {
        private readonly IDictionary<string, Message> _dmrRequests;
        private readonly IChatService _chatService;

        public DmrService(IChatService chatService)
        {
            _dmrRequests = new ConcurrentDictionary<string, Message>();
            _chatService = chatService;
        }

        public void AddDmrMessage(HeadersInput? headersInput)
        {
            if (headersInput == null || headersInput.XMessageIdRef == null)
            {
                return;
            }

            var message = _dmrRequests[headersInput.XMessageIdRef];
            _chatService.AddMessageMetadata(message, headersInput);
        }
    }
}