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
        public void GivenASerializer_WhenDeserializingIsOn_ThenWeGetSettings()
        {
            var serializer = new StrategySettingsSerializer();
            var json = "{ \"IsOn\":true}";
            var strategy = serializer.Deserialize(StrategyNames.IsOn, json);

            strategy.Should().BeOfType<IsOnStrategy>();
        }
        
        [Test]
        public void GivenASerializer_WhenDeserializingIsInList_ThenWeGetSettings()
        {
            var serializer = new StrategySettingsSerializer();
            var json = "{ \"IsInLits\":[\"a\",\"b\"]}";
            var strategy = serializer.Deserialize(StrategyNames.IsInList, json);

            strategy.Should().BeOfType<IsInListStrategy>();
        }
        
        [Test]
        public void GivenASerializer_WhenDeserializingIsGreaterThan_ThenWeGetSettings()
        {
            var serializer = new StrategySettingsSerializer();
            var json = "{ \"Value\": 5 }";
            var strategy = serializer.Deserialize(StrategyNames.IsGreaterThan, json);

            strategy.Should().BeOfType<IsGreaterThanStrategy>();
        }
        
        
        [Test]
        public void GivenASerializer_WhenDeserializingIsLowerThan_ThenWeGetSettings()
        {
            var serializer = new StrategySettingsSerializer();
            var json = "{ \"Value\": 5 }";
            var strategy = serializer.Deserialize(StrategyNames.IsLowerThan, json);

            strategy.Should().BeOfType<IsLowerThanStrategy>();
        }
    }
}