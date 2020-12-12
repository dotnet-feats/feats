using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Feats.Domain.Strategies;
using Microsoft.Extensions.Logging;

namespace Feats.Evaluations.Strategies
{
    public class IsGreaterThanStrategyEvaluator : IEvaluateStrategy<IsGreaterThanStrategy>
    {
        private readonly ILogger<IsGreaterThanStrategyEvaluator> _logger;

        public IsGreaterThanStrategyEvaluator(ILogger<IsGreaterThanStrategyEvaluator> logger)
        {
            this._logger = logger;
        }

        public Task<bool> IsOn(IsGreaterThanStrategy strategy, IDictionary<string, string> values = null)
        {
            if (values.ContainsKey(StrategySettings.GreaterThan))
            {
                try
                {
                    var myStrategyValue =
                        Convert.ToDouble(values[StrategySettings.GreaterThan], CultureInfo.InvariantCulture);

                    return Task.FromResult(myStrategyValue > strategy.Settings.Value);
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