using RequestProcessor.AsyncProcessor;
using System.Diagnostics.CodeAnalysis;

namespace MockBot.Api.Services.Dmr
{
    /// <summary>
    /// A settings object for <see cref="DmrService"/>
    /// </summary>
    [ExcludeFromCodeCoverage] // No logic so not appropriate for code coverage
    public class DmrServiceSettings : AsyncProcessorSettings
    {
        /// <summary>
        /// The base URI for the DMR REST API
        /// </summary>
        public Uri DmrApiUri { get; set; }
    }
}