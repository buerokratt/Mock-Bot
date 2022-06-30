using System.Diagnostics.CodeAnalysis;

namespace MockBot.Api.Models
{
    // No logic so no unit tests are required
    [ExcludeFromCodeCoverage]
    public static class Constants
    {
        public const string Unknown = "Unknown";
        public const string XSendToDmr = "Dmr";
        public const string XSendToClassifier = "Classifier";
        public const string XModelType = "application/vnd.classifier.classification+json;version=1";
        public const string ContentTypePlain = "text/plain";
        public const string PostNoBodyMessage = "Post must have a body";
    }
}
