using System.Collections.Generic;
using Feats.CQRS.Queries;

namespace Feats.Management.Paths.Queries 
{
    public class GetAllPathsQuery : IQuery<IEnumerable<PathAndFeatureCount>>
    {
        public string Filter { get; set; }
    }

    public class GetAllPathsQueryHandler : IHandleQuery<GetAllPathsQuery, IEnumerable<PathAndFeatureCount>>
    {
        public System.Threading.Tasks.Task<IEnumerable<PathAndFeatureCount>> Handle(GetAllPathsQuery query)
        {
            throw new System.NotImplementedException();
        }
    }

    public class PathAndFeatureCount
    {
        public string Path { get;set; }

        public int TotalFeatures { get; set; }
    }
}