using System;

namespace Feats.Domain.Strategies
{
    public interface IStrategy
    {
        string Name { get; }
    }

    public interface IStrategy<TStrategySettings> : IStrategy
        where TStrategySettings : IStrategySettings
    {

        TStrategySettings Settings { get; }
    }
}