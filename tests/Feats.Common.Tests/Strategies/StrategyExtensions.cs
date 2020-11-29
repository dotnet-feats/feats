
using System.Collections.Generic;
using System.Text.Json;
using Feats.Domain.Strategies;

namespace Feats.Common.Tests
{
    public static class StrategyExtensions 
    {
        public static IDictionary<string, string> GivenDefaultStrategies(this TestBase testClass)
        {
            var json = JsonSerializer.Serialize(
                new IsEnabledStrategySettings{
                    IsOn = true,
                });
                
            return new Dictionary<string, string>
            {
                { StrategyNames.IsEnabled, json },
            };
        }
    }
}