using System;
using System.Diagnostics.CodeAnalysis;

namespace Feats.Domain.Strategies
{
    [ExcludeFromCodeCoverage]
    public sealed class IsBeforeStrategy : IStrategy<DateTimeOffsetStrategySettings>
    {
        public string Name => StrategyNames.IsBefore;

        public DateTimeOffsetStrategySettings Settings { get; set; }
    }
    
    [ExcludeFromCodeCoverage]
    public sealed class DateTimeOffsetStrategySettings : IStrategySettings
    {
        public DateTimeOffset Value { get; set; }
    }
}