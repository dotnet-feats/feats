using Feats.Domain;
using Feats.Domain.Exceptions;

namespace Feats.EventStore.Exceptions
{
    public class FeatureNotFoundException : NotFoundException
    {
        public FeatureNotFoundException(string path, string name)
            : base($"No Feature could be found with path {path} & name {name}")
        {
        }
    }
}
