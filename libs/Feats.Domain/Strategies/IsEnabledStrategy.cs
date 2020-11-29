using System;

namespace Feats.Domain.Strategies
{
    public interface IIsOnStrategy : IStrategy<IsOnStrategySettings>
    {
    }

    public class IsOnStrategy : IIsOnStrategy
    {
        public string Name => StrategyNames.IsOn;

        public IsOnStrategySettings Settings { get; set; }
    }

    public class IsOnStrategySettings : IStrategySettings
    {
        public bool IsOn { get; set; }
    }
}