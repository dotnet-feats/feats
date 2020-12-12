using Feats.Domain;
using Feats.Domain.Exceptions;

namespace Feats.Management.Features.Exceptions
{
    public sealed class FeatureNotFoundException : NotFoundException
    {
        public FeatureNotFoundException()
        :base()
        {}
        
        public FeatureNotFoundException(string path, string name)
            : base($"No Feature could be found with path {path} & name {name}")
        {
        }
    }
}
