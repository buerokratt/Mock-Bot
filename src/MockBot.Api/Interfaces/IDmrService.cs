using MockBot.Api.Models;

namespace MockBot.Api.Interfaces
{
    public interface IDmrService
    {
        void AddDmrMessage(HeadersInput headersInput);
    }
}