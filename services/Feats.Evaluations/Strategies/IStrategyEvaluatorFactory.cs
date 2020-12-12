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
        
        private readonly IEvaluateStrategy<IsInListStrategy> _isInListEvaluator;
        
        private readonly IEvaluateStrategy<IsGreaterThanStrategy> _isGreaterThanEvaluator;
        
        private readonly IEvaluateStrategy<IsLowerThanStrategy> _isLowerThanEvaluator;

        public StrategyEvaluatorFactory(
            IStrategySettingsSerializer strategySerializer,
            IEvaluateStrategy<IsOnStrategy> isOnEvaluator,
            IEvaluateStrategy<IsInListStrategy> isInListEvaluator,
            IEvaluateStrategy<IsGreaterThanStrategy> isGreaterThanEvaluator,
            IEvaluateStrategy<IsLowerThanStrategy> isLowerThanEvaluator)
        {
            this._strategySerializer = strategySerializer;
            this._isOnEvaluator = isOnEvaluator;
            this._isInListEvaluator = isInListEvaluator;
            this._isGreaterThanEvaluator = isGreaterThanEvaluator;
            this._isLowerThanEvaluator = isLowerThanEvaluator;
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
                    
                case IsInListStrategy isInList:
                    return await this._isInListEvaluator.IsOn(isInList, values);
                    
                case IsGreaterThanStrategy isGreaterThan:
                    return await this._isGreaterThanEvaluator.IsOn(isGreaterThan, values);
                
                case IsLowerThanStrategy isLowerThan:
                    return await this._isLowerThanEvaluator.IsOn(isLowerThan, values);

                default:
                    throw new NotImplementedException("the requested strategy has not been implemented yet");
            }
        }
    }
}