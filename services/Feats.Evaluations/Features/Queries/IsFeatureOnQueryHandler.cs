using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Feats.CQRS.Queries;
using Feats.Domain;
using Feats.Evaluations.Features.Exceptions;
using Feats.Evaluations.Strategies;

namespace Feats.Evaluations.Features.Queries
{
    public class IsFeatureOnQuery : IQuery<bool>
    {
        public string Path { get; set; }
        
        public string Name { get; set; }
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
            var combined = System.IO.Path.Combine(query.Path, query.Name);
            var feature = this._featuresAggregate.Features
                .Where(_ => _.State == FeatureState.Published)
                .Where(_ => System.IO.Path.Combine(_.Path, _.Name).Equals(combined, StringComparison.InvariantCultureIgnoreCase))
                .FirstOrDefault();

            if (feature == null)
            {
                throw new FeatureNotPublishedExeption();
            }

            var results = new List<bool>();
            foreach(var strategyInFeature in feature.Strategies)
            {
                var result = await this._strategyEvaluator.IsOn(
                    strategyInFeature.Key,
                    strategyInFeature.Value);
                
                results.Add(result);
            }

            return results.All(_ => _);
        }
    }
}