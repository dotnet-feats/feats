using System;
using System.Diagnostics.CodeAnalysis;

namespace Feats.Domain.Strategies
{
    [ExcludeFromCodeCoverage]
    public sealed class IsGreaterThanStrategy : IStrategy<NumericalStrategySettings>
    {
        public string Name => StrategyNames.IsGreaterThan;

        public NumericalStrategySettings Settings { get; set; }
    }
    
    [ExcludeFromCodeCoverage]
    public sealed class NumericalStrategySettings : IStrategySettings
    {
        public double Value { get; set; }
    }
}