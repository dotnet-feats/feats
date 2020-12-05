using Feats.Domain;

namespace Feats.Management.Features.Exceptions
{
    public class FeatureNotFoundException : NotFoundException
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
