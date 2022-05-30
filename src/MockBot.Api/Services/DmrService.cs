using System.Collections.Concurrent;
using MockBot.Api.Interfaces;
using MockBot.Api.Models;

namespace MockBot.Api.Services
{
    public class DmrService : IDmrService
    {
        public IDictionary<string, Message> DmrRequests { get; }

        public DmrService()
        {
            DmrRequests = new ConcurrentDictionary<string, Message>();
        }

        public void AddDmrRequest(Message? message)
        {
            if (message == null)
            {
                return;
            }

            DmrRequests.Add(message.Id.ToString(), message);
        }
    }
}