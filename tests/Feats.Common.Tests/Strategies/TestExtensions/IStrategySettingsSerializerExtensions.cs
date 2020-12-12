using System.Diagnostics.CodeAnalysis;
using Feats.Domain.Strategies;

namespace Feats.Common.Tests.Strategies.TestExtensions
{
    [ExcludeFromCodeCoverage]
    public static class IStrategySettingsSerializerExtensions 
    {
        public static IStrategySettingsSerializer GivenIStrategySettingsSerializer(this TestBase testClass)
        {
            return new StrategySettingsSerializer();
        }
    }
}