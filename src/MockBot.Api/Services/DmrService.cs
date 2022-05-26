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

        public void AddDmrMessage(string? XSentBy, string? XSendTo, string? XMessageId, string? XMessageIdRef)
        {
            if (XMessageIdRef == null)
            {
                return;
            }

            var message = _dmrRequests[XMessageIdRef];
            _chatService.AddMessageMetadata(message, XSentBy, XSendTo, XMessageId, XMessageIdRef);
        }
    }
}