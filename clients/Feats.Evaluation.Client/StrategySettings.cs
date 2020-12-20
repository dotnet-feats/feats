using System.Diagnostics.CodeAnalysis;

namespace Feats.Evaluation.Client
{
    [ExcludeFromCodeCoverage]
    internal static class StrategySettings
    {
        public const string List = "feats.list";
        
        public const string Before = "feats.before";
        
        public const string After = "feats.after";
        
        public const string GreaterThan = "feats.greater";
        
        public const string LowerThan = "feats.lower";
    }
}