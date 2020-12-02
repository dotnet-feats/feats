
using Feats.Domain.Strategies;
using FluentAssertions;
using NUnit.Framework;

namespace Feats.Evaluations.Tests.Strategies
{
    public class IStrategySettingsSerializerTests
    {
        [Test]
        public void GivenASerializer_WhenSerializing_ThenWeGetJsonString()
        {
            var serializer = new StrategySettingsSerializer();
            var json = "{\"IsOn\":true}";
            var results = serializer.Serialize(new IsOnStrategySettings { IsOn = true });

            results.Should().Be(json);
        }

        [Test]
        public void GivenASerializer_WhenDeserializing_ThenWeGetSettings()
        {
            var serializer = new StrategySettingsSerializer();
            var json = "{ \"IsOn\":true}";
            var stragety = serializer.Deserialize(StrategyNames.IsOn, json);

            stragety.Should().BeOfType<IsOnStrategy>();
        }
    }
}