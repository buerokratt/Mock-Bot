namespace MockBot.Api.Controllers.Extensions
{
    public static class LoggerExtensions
    {
        private static readonly Action<ILogger, string, string, string, Exception> _dmrCallbackReceived =
            LoggerMessage.Define<string, string, string>(
                LogLevel.Information,
                new EventId(1, nameof(DmrCallbackReceived)),
                "Received Dmr Callback from '{From}' {Encoded} {Decoded}");

        public static void DmrCallbackReceived(this ILogger logger, string from, string encodedPayload, string decodedPayload)
        {
            _dmrCallbackReceived(logger, from, encodedPayload, decodedPayload, null);
        }
    }
}