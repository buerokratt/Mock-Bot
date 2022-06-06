using System.Diagnostics.CodeAnalysis;

namespace MockBot.Api.Services.Dmr.Extensions
{
    [ExcludeFromCodeCoverage] // Placeholder until we get the NuGet package
    public static class LoggerExtensions
    {
        private static readonly Action<ILogger, string, string, Exception> _dmrCallback =
            LoggerMessage.Define<string, string>(
                LogLevel.Information,
                new EventId(1, "DmrCallbackPosted"),
                "Callback to DMR with classification = '{Classification}', message = '{Message}'");

        private static readonly Action<ILogger, Exception> _dmrCallbackFailed =
            LoggerMessage.Define(
                LogLevel.Error,
                new EventId(2, "DmrCallbackFailed"),
                "Callback to DMR failed");

        public static void DmrCallback(this ILogger logger, string? classification, string? message)
        {
            classification ??= string.Empty;
            message ??= string.Empty;
            _dmrCallback(logger, classification, message, new ArgumentException(message));
        }

        public static void DmrCallbackFailed(this ILogger logger, Exception exception)
        {
            _dmrCallbackFailed(logger, exception);
        }
    }
}