using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Feats.CQRS.Queries;
using Feats.Evaluations.Strategies;
using Feats.Management.Features;

namespace Feats.Evaluations.Features.Queries
{
    public class IsFeatureOnQuery : IQuery<bool>
    {
        public string PathAndName { get; set; }
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

            var feature = this._featuresAggregate.Features
                .Where(_ => Path.Combine(_.Path, _.Name).Equals(query.PathAndName))
                .FirstOrDefault();

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