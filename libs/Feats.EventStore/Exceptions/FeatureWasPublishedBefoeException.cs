using Feats.Domain;

namespace Feats.EventStore.Exceptions
{
    public class FeatureWasPublishedBeforeException : ValidationException
    {        
        public FeatureWasPublishedBeforeException()
            : base($"A once published feature cannot be modified")
        {
        }
    }
}
