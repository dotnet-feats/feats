using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Feats.Domain.Strategies
{
    [ExcludeFromCodeCoverage]
    public sealed class IsInListStrategy : IStrategy<IsInListStrategySettings>
    {
        public string Name => StrategyNames.IsInList;

        public IsInListStrategySettings Settings { get; set; }
    }
    
    [ExcludeFromCodeCoverage]
    public sealed class IsInListStrategySettings : IStrategySettings
    {
        public IsInListStrategySettings()
        {
            this.ListName = "feats.list"; //this is the default value, but can be overridden by users
        }

        public string ListName { get; set; }
        
        public IEnumerable<string> Items { get; set; }
    }
}