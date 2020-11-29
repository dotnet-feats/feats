using System;

namespace Feats.Domain.Strategies
{
    public interface IIsEnabledStrategy : IStrategy<IsEnabledStrategySettings>
    {
    }

    public class IsEnabledStrategy : IIsEnabledStrategy
    {
        public string Name => StrategyNames.IsEnabled;

        public IsEnabledStrategySettings Settings { get; set; }
    }

    public class IsEnabledStrategySettings : IStrategySettings
    {
        public bool IsOn { get; set; }
    }
}