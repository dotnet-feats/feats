using Feats.Domain;
using Feats.Domain.Exceptions;

namespace Feats.EventStore.Exceptions
{
    public class FeatureIsNotInDraftException : ValidationException
    {        
        public FeatureIsNotInDraftException()
            : base($"A once published feature cannot be modified")
        {
        }
    }
}
