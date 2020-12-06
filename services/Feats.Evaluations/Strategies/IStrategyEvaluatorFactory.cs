using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Feats.Domain.Strategies;

namespace Feats.Evaluations.Strategies
{
    public interface IStrategyEvaluatorFactory
    {
        Task<bool> IsOn(
            string strategyName, 
            string serializedStrategySettings,
            IDictionary<string, string> values = null);
    }

    public class StrategyEvaluatorFactory : IStrategyEvaluatorFactory
    {
        private readonly IStrategySettingsSerializer _strategySerializer;

        private readonly IEvaluateStrategy<IsOnStrategy> _isOnEvaluator;

        public StrategyEvaluatorFactory(
            IStrategySettingsSerializer strategySerializer,
            IEvaluateStrategy<IsOnStrategy> isOnEvaluator)
        {
            this._strategySerializer = strategySerializer;
            this._isOnEvaluator = isOnEvaluator;
        }

        public async Task<bool> IsOn(
            string strategyName, 
            string serializedStrategySettings,
            IDictionary<string, string> values = null)
        {
            var strategy = this._strategySerializer.Deserialize(strategyName, serializedStrategySettings);
            
            return await this.IsOn(strategy, values);
        }

        private async Task<bool> IsOn(
            IStrategy strategy,
            IDictionary<string, string> values = null)
        {
            switch(strategy)
            {
                case IsOnStrategy isOn:
                    return await this._isOnEvaluator.IsOn(isOn);
                default:
                    throw new NotImplementedException("the requested strategy has not been implemented yet");
            }
        }
    }
}