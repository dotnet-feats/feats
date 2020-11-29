using System;

namespace Feats.Domain.Strategies
{
    public class IsOnStrategy : IStrategy<IsOnStrategySettings>
    {
        public string Name => StrategyNames.IsOn;

        public IsOnStrategySettings Settings { get; set; }
    }

    public class IsOnStrategySettings : IStrategySettings
    {
        public bool IsOn { get; set; }
    }
}