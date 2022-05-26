using MockBot.Api.Models;

namespace MockBot.Api.Interfaces
{
    public interface IDmrService
    {
        void AddDmrMessage(string? XSentBy, string? XSendTo, string? XMessageId, string? XMessageIdRef);
    }
}