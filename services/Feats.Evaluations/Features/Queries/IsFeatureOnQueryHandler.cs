using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Feats.CQRS.Queries;
using Feats.Domain;
using Feats.Domain.Aggregates;
using Feats.Evaluations.Features.Exceptions;
using Feats.Evaluations.Strategies;

namespace Feats.Evaluations.Features.Queries
{
    public class IsFeatureOnQuery : IQuery<bool>
    {
        public string Path { get; set; }
        
        public string Name { get; set; }
        
        public IDictionary<string, string> Values { get; set; }
    }

    public class IsFeatureOnQueryHandler : IHandleQuery<IsFeatureOnQuery, bool>
    {
        private readonly IFeaturesAggregate _featuresAggregate;

        private readonly IStrategyEvaluatorFactory _strategyEvaluator;

        public IsFeatureOnQueryHandler(
            IFeaturesAggregate featuresAggregate,
            IStrategyEvaluatorFactory strategyEvaluator)
        {
            this._featuresAggregate = featuresAggregate;
            this._strategyEvaluator = strategyEvaluator;
        }

        public async Task<bool> Handle(IsFeatureOnQuery query)
        {
            await this._featuresAggregate.Load();
            var combined = PathHelper.CombineNameAndPath(query.Path, query.Name);
            var feature = this._featuresAggregate.Features
                .Where(_ => _.State == FeatureState.Published)
                .Where(_ => PathHelper.CombineNameAndPath(_.Path, _.Name).Equals(combined, StringComparison.InvariantCultureIgnoreCase))
                .FirstOrDefault();

            if (feature == null)
            {
                throw new FeatureNotPublishedException();
            }

            var results = new List<bool>();
            foreach(var strategy in feature.Strategies)
            {
                var result = await this._strategyEvaluator.IsOn(
                    strategy.Name,
                    strategy.Value,
                    query.Values);
                
                results.Add(result);
            }

            return results.All(_ => _);
        }
    }
}