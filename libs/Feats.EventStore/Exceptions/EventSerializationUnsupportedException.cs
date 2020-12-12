using Feats.Domain;
using Feats.Domain.Exceptions;

namespace Feats.EventStore.Exceptions
{
    public class EventSerializationUnsupportedException : ValidationException
    {        
        public EventSerializationUnsupportedException()
            : base("An event without its EventData transformation was submitted, cannot process event.")
        {
        }
    }
}