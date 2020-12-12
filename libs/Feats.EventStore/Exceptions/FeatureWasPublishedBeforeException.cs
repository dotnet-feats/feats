using Feats.Domain;
using Feats.Domain.Exceptions;

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
