namespace Feats.Domain.Strategies
{
    public interface IStrategySettings
    {
    }

    public static class StrategySettings
    {
        public const string List = "feats.list";
        
        public const string Date = "feats.date";
        
        public const string Number = "feats.int";
    }
}