using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Feats.Domain.Strategies;
using Microsoft.Extensions.Logging;

namespace Feats.Evaluations.Strategies
{
    public class IsLowerThanStrategyEvaluator : IEvaluateStrategy<IsLowerThanStrategy>
    {
        private readonly ILogger<IsLowerThanStrategyEvaluator> _logger;

        public IsLowerThanStrategyEvaluator(ILogger<IsLowerThanStrategyEvaluator> logger)
        {
            this._logger = logger;
        }

        public Task<bool> IsOn(IsLowerThanStrategy strategy, IDictionary<string, string> values = null)
        {
            if (values.ContainsKey(StrategySettings.LowerThan))
            {
                try
                {
                    var myStrategyValue =
                        Convert.ToDouble(values[StrategySettings.LowerThan], CultureInfo.InvariantCulture);

                    return Task.FromResult(myStrategyValue < strategy.Settings.Value);
                }
                catch (Exception e)
                {
                    this._logger.LogError(e, "Failed to convert string to numerical values. Please use a standard, non-culture specific numerical format.");
                }
            }

            // todo: might wanna throw up instead
            return Task.FromResult(false);
        }
    }
}