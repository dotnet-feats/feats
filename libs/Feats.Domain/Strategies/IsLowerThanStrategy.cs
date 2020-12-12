using System;
using System.Diagnostics.CodeAnalysis;

namespace Feats.Domain.Strategies
{
    [ExcludeFromCodeCoverage]
    public sealed class IsLowerThanStrategy : IStrategy<NumericalStrategySettings>
    {
        public string Name => StrategyNames.IsLowerThan;

        public NumericalStrategySettings Settings { get; set; }
    }
}