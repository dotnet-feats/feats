using System;
using System.Diagnostics.CodeAnalysis;

namespace Feats.Domain.Strategies
{
    [ExcludeFromCodeCoverage]
    public sealed class IsOnStrategy : IStrategy<IsOnStrategySettings>
    {
        public string Name => StrategyNames.IsOn;

        public IsOnStrategySettings Settings { get; set; }
    }
    
    [ExcludeFromCodeCoverage]
    public sealed class IsOnStrategySettings : IStrategySettings
    {
        public bool IsOn { get; set; }
    }
}