using System.Collections.Generic;
using System.Threading.Tasks;
using Feats.Domain.Strategies;

namespace Feats.Evaluations.Strategies
{
    public class IsOnStrategyEvaluator : IEvaluateStrategy<IsOnStrategy>
    {
        public Task<bool> IsOn(IsOnStrategy strategy, IDictionary<string, string> values = null)
        {
            return Task.FromResult(strategy.Settings.IsOn);
        }
    }
}