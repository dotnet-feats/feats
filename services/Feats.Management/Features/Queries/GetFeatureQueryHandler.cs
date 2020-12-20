using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Feats.CQRS.Queries;
using Feats.Domain;
using Feats.EventStore.Aggregates;
using Feats.Management.Features.Exceptions;

namespace Feats.Management.Features.Queries 
{
    public class GetFeatureQuery : IQuery<FeatureAndStrategyConfiguration>
    {
        public string Path { get; set; }

        public string Name { get; set; }
    }

    public class GetFeatureQueryHandler : IHandleQuery<GetFeatureQuery, FeatureAndStrategyConfiguration>
    {
        private readonly IFeaturesAggregate _featuresAggregate;

        public GetFeatureQueryHandler(
            IFeaturesAggregate featuresAggregate)
        {
            this._featuresAggregate = featuresAggregate;
        }

        public async Task<FeatureAndStrategyConfiguration> Handle(GetFeatureQuery query)
        {
            await this._featuresAggregate.Load();
            var combined = PathHelper.CombineNameAndPath(query.Path, query.Name);

            var first = this._featuresAggregate.Features
                .FirstOrDefault(_ => PathHelper.CombineNameAndPath(_.Path, _.Name).Equals(combined, StringComparison.InvariantCultureIgnoreCase));
            
            if (first == null)
            {
                throw new FeatureNotFoundException(query.Path, query.Name);
            }

            return new FeatureAndStrategyConfiguration
            {
                Feature = first.Name,
                Path = first.Path,
                State = first.State,
                Strategies = first.Strategies,
                CreatedBy = first.CreatedBy,
                CreatedOn = first.CreatedOn,
                UpdatedOn = first.UpdatedOn,
            };
        }
    }

    public class FeatureAndStrategyConfiguration
    {
        public string Feature { get;set; }

        public string Path { get;set; }

        public IEnumerable<IFeatureStrategy> Strategies { get; set; }

        public FeatureState State { get; set; }

        public DateTimeOffset CreatedOn {get; set;}

        public DateTimeOffset UpdatedOn {get; set;}

        public string CreatedBy {get; set;}
    }
}