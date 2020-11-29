using System.Threading.Tasks;
using Feats.Domain.Strategies;

namespace Feats.Evaluations.Strategies
{
    public class IsEnabledStrategyEvaluator : IEvaluateStrategy<IsEnabledStrategy>
    {
        public Task<bool> IsOn(IsEnabledStrategy strategy)
        {
            return Task.FromResult(strategy.Settings.IsOn);
        }
    }
}