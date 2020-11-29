using System;
using System.Text.Json;

namespace Feats.Domain.Strategies
{
    public interface IStrategySettingsSerializer
    {
        string Serialize<TStrategySettings>(TStrategySettings settings) 
            where TStrategySettings : IStrategySettings;

        IStrategy Deserialize(string strategyName, string strategySettings);

    }

    public class StrategySettingsSerializer : IStrategySettingsSerializer
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