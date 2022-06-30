namespace MockBot.Api.Controllers.Extensions
{
    public static class LoggerExtensions
    {
        private static readonly Action<ILogger, string, string, string, string, Exception> _dmrCallbackReceived =
            LoggerMessage.Define<string, string, string, string>(
                LogLevel.Information,
                new EventId(1, nameof(DmrCallbackReceived)),
                "Received Dmr Callback from '{From}' regarding '{MessageRef}' {Encoded} {Decoded}");

        private static readonly Action<ILogger, string, string, string, string, Exception> _messageWithNoChatReceived =
            LoggerMessage.Define<string, string, string, string>(
                LogLevel.Warning,
                new EventId(1, nameof(DmrCallbackReceived)),
                "Received a Message from Dmr where the ChatId ('{NotFoundChatid}' could not be found. Have created a new chat ('{NewChatId}') and assigned the Message to it. {Encoded} {Decoded}");

        public static void DmrCallbackReceived(this ILogger logger, string from, string messageRef, string encodedPayload, string decodedPayload)
        {
            _dmrCallbackReceived(logger, from, messageRef, encodedPayload, decodedPayload, null);
        }

        public static void MessageWithNoChatReceived(this ILogger logger, string notFoundChatid, string newChatId, string encodedPayload, string decodedPayload)
        {
            _messageWithNoChatReceived(logger, notFoundChatid, newChatId, encodedPayload, decodedPayload, null);
        }
    }
}