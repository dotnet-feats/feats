using System;
using System.Threading.Tasks;
using Feats.Domain.Strategies;

namespace Feats.Evaluations.Strategies
{
    public interface IStrategyEvaluatorFactory
    {
        Task<bool> IsOn(IStrategy strategy);
    }

    public class StrategyEvaluatorFactory : IStrategyEvaluatorFactory
    {
        private readonly IEvaluateStrategy<IsOnStrategy> _isOnEvaluator;

        public StrategyEvaluatorFactory(
            IEvaluateStrategy<IsOnStrategy> isOnEvaluator)
        {
            this._isOnEvaluator = isOnEvaluator;
        }

        public async Task<bool> IsOn(IStrategy strategy)
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