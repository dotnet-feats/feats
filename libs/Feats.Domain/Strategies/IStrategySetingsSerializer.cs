using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace Feats.Domain.Strategies
{
    public interface IStrategySettingsSerializer
    {
        string Serialize<TStrategySettings>(TStrategySettings settings) 
            where TStrategySettings : IStrategySettings;

        IStrategy Deserialize(string strategyName, string strategySettings);

    }

    [ExcludeFromCodeCoverage]
    public sealed class StrategySettingsSerializer : IStrategySettingsSerializer
    {
        public IStrategy Deserialize(string strategyName, string strategySettings)
        {
            switch (strategyName)
            {
                case StrategyNames.IsOn:
                    return new IsOnStrategy
                    {
                        Settings = JsonSerializer.Deserialize<IsOnStrategySettings>(strategySettings)
                    };
                    
                case StrategyNames.IsInList:
                    return new IsInListStrategy
                    {
                        Settings = JsonSerializer.Deserialize<IsInListStrategySettings>(strategySettings)
                    };
                    
                case StrategyNames.IsGreaterThan:
                    return new IsGreaterThanStrategy
                    {
                        Settings = JsonSerializer.Deserialize<NumericalStrategySettings>(strategySettings)
                    };
                
                case StrategyNames.IsLowerThan:
                    return new IsLowerThanStrategy
                    {
                        Settings = JsonSerializer.Deserialize<NumericalStrategySettings>(strategySettings)
                    };
                    
                case StrategyNames.IsAfter:
                    return new IsAfterStrategy()
                    {
                        Settings = JsonSerializer.Deserialize<DateTimeOffsetStrategySettings>(strategySettings)
                    };
                
                case StrategyNames.IsBefore:
                    return new IsBeforeStrategy()
                    {
                        Settings = JsonSerializer.Deserialize<DateTimeOffsetStrategySettings>(strategySettings)
                    };
                
                default: 
                    throw new NotImplementedException($"Requested settings don't exists for strategy {strategyName}");
            }
        }

        public string Serialize<TStrategySettings>(TStrategySettings settings) 
            where TStrategySettings : IStrategySettings
        {
            return JsonSerializer.Serialize(settings);
        }
    }
}