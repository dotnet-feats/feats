using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Feats.Domain.Strategies;
using Microsoft.Extensions.Logging;

namespace Feats.Evaluations.Strategies
{
    public class IsAfterStrategyEvaluator : IEvaluateStrategy<IsAfterStrategy>
    {
        private readonly ILogger<IsAfterStrategyEvaluator> _logger;

        public IsAfterStrategyEvaluator(ILogger<IsAfterStrategyEvaluator> logger)
        {
            this._logger = logger;
        }

        public Task<bool> IsOn(IsAfterStrategy strategy, IDictionary<string, string> values = null)
        {
            if (values.ContainsKey(StrategySettings.After))
            {
                try
                {
                    var myStrategyValue =
                        DateTimeOffset.Parse(values[StrategySettings.After], CultureInfo.InvariantCulture);

                    return Task.FromResult(myStrategyValue > strategy.Settings.Value);
                }
                catch (Exception e)
                {
                    this._logger.LogError(e, "Failed to convert string to a date value. Please use a standard, non-culture specific date format.");
                }
            }

            // todo: might wanna throw up instead
            return Task.FromResult(false);
        }
    }
}