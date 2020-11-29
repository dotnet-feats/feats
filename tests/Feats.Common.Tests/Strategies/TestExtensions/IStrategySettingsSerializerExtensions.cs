using Feats.Domain.Strategies;

namespace Feats.Common.Tests.Strategies
{
    public static class IStrategySettingsSerializerExtensions 
    {
        public static IStrategySettingsSerializer GivenIStrategySettingsSerializer(this TestBase testClass)
        {
            return new StrategySettingsSerializer();
        }
    }
}