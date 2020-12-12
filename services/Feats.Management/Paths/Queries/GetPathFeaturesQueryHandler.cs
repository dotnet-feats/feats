using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Feats.CQRS.Queries;
using Feats.Domain;
using Feats.EventStore.Aggregates;
using Feats.Management.Features;

namespace Feats.Management.Paths.Queries 
{
    public class GetPathFeaturesQuery : IQuery<IEnumerable<FeatureAndStrategy>>
    {
        public string Path { get; set; }
    }

    public class GetPathFeaturesQueryHandler : IHandleQuery<GetPathFeaturesQuery, IEnumerable<FeatureAndStrategy>>
    {
        private readonly IFeaturesAggregate _featuresAggregate;

        public GetPathFeaturesQueryHandler(
            IFeaturesAggregate featuresAggregate)
        {
            this._featuresAggregate = featuresAggregate;
        }

        public async Task<IEnumerable<FeatureAndStrategy>> Handle(GetPathFeaturesQuery query)
        {
            await this._featuresAggregate.Load();

            if(string.IsNullOrEmpty(query.Path))
            {
                return this._featuresAggregate.Features
                    .Select(_ => new FeatureAndStrategy
                    {
                        Feature = _.Name,
                        Path = _.Path,
                        State = _.State,
                        StrategyNames = _.Strategies.Select(a => a.Key),
                        CreatedBy = _.CreatedBy,
                        CreatedOn = _.CreatedOn,
                        UpdatedOn = _.UpdatedOn,
                    });
            }

            return this._featuresAggregate.Features
                .Where(_ => _.Path.StartsWith(query.Path, StringComparison.InvariantCultureIgnoreCase))
                .Select(_ => new FeatureAndStrategy
                {
                    Feature = _.Name,
                    Path = _.Path,
                    State = _.State,
                    StrategyNames = _.Strategies.Select(a => a.Key),
                    CreatedBy = _.CreatedBy,
                    CreatedOn = _.CreatedOn,
                    UpdatedOn = _.UpdatedOn,
                });
        }
    }

    public class FeatureAndStrategy
    {
        public string Feature { get;set; }

        public string Path { get;set; }

        public IEnumerable<string> StrategyNames { get; set; }

        public FeatureState State { get; set; }

        public DateTimeOffset CreatedOn {get; set;}

        public DateTimeOffset UpdatedOn {get; set;}

        public string CreatedBy {get; set;}
    }
}