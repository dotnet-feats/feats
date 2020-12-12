using System;
using System.Diagnostics.CodeAnalysis;

namespace Feats.Domain.Strategies
{
    [ExcludeFromCodeCoverage]
    public sealed class IsAfterStrategy : IStrategy<DateTimeOffsetStrategySettings>
    {
        public string Name => StrategyNames.IsAfter;

        public DateTimeOffsetStrategySettings Settings { get; set; }
    }
}