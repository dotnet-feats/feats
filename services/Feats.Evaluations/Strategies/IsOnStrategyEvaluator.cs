using System.Threading.Tasks;
using Feats.Domain.Strategies;

namespace Feats.Evaluations.Strategies
{
    public class IsOnStrategyEvaluator : IEvaluateStrategy<IsOnStrategy>
    {
        public string StrategyName => StrategyNames.IsOn;

        public Task<bool> IsOn(IsOnStrategy strategy)
        {
            return Task.FromResult(strategy.Settings.IsOn);
        }
    }
}