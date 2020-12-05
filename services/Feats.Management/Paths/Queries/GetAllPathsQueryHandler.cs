using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Feats.CQRS.Queries;

namespace Feats.Management.Paths.Queries 
{
    public class GetAllPathsQuery : IQuery<IEnumerable<PathAndFeatureCount>>
    {
        public string Filter { get; set; }
    }

    public class GetAllPathsQueryHandler : IHandleQuery<GetAllPathsQuery, IEnumerable<PathAndFeatureCount>>
    {
        private readonly IPathsAggregate _aggregate;

        public GetAllPathsQueryHandler(IPathsAggregate aggregate)
        {
            this._aggregate = aggregate;
        }

        public async Task<IEnumerable<PathAndFeatureCount>> Handle(GetAllPathsQuery query)
        {
            await this._aggregate.Load();

            if (string.IsNullOrEmpty(query.Filter))
            {
                return this._aggregate.Paths
                    .Select(_ => new PathAndFeatureCount {
                        Path = _.Name,
                        TotalFeatures = _.TotalFeatures,
                    });
            }
            
            return this._aggregate.Paths
                .Where(_ => _.Name.Contains(query.Filter, StringComparison.InvariantCultureIgnoreCase))
                .Select(_ => new PathAndFeatureCount {
                    Path = _.Name,
                    TotalFeatures = _.TotalFeatures,
                });
        }
    }

    public class PathAndFeatureCount
    {
        public string Path { get;set; }

        public int TotalFeatures { get; set; }
    }
}