using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Feats.Domain.Strategies;

namespace Feats.Evaluations.Strategies
{
    public class IsInListStrategyEvaluator : IEvaluateStrategy<IsInListStrategy>
    {
        public Task<bool> IsOn(
            IsInListStrategy strategy, 
            IDictionary<string, string> values = null)
        {
            if (values.ContainsKey(strategy.Settings.ListName))
            {
                var myStrategyValues = values[strategy.Settings.ListName];

                return Task.FromResult(strategy.Settings.Items.Contains(myStrategyValues));
            }

            // todo: might wanna throw up instead
            return Task.FromResult(false);
        }
    }
}