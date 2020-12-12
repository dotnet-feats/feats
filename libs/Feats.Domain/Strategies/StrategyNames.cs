using System;
using System.Diagnostics.CodeAnalysis;

namespace Feats.Domain.Strategies
{
    [ExcludeFromCodeCoverage]
    public static class StrategyNames
    {
        public const string IsOn = "IsOn";
        
        public const string IsInList = "IsInList";
        
        public const string IsGreaterThan = "IsGreaterThan";
        
        public const string IsLowerThan = "IsLowerThan";
        
        public const string IsBefore = "IsBefore";
        
        public const string IsAfter = "IsAfter";
    }
}