using System.Collections.Generic;
using Feats.Common.Tests;
using Feats.Domain.Strategies;
using FluentAssertions;
using NUnit.Framework;

namespace Feats.Evaluations.Tests.Strategies
{
    public class IStrategySettingsSerializerTests : TestBase
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
            var json = "{ \"ListName\" : \"patate\", \"Items\": [\"a\"] }";
            var strategy = serializer.Deserialize(StrategyNames.IsInList, json);

            strategy.Should().BeOfType<IsInListStrategy>();
            var expectedStrategy = strategy as IsInListStrategy;
            expectedStrategy.Settings.ListName.Should().Be("patate");
            expectedStrategy.Settings.Items.Should().BeEquivalentTo(new List<string> { "a" });
        }
        
        [Test]
        public void GivenASerializer_WhenDeserializingIsGreaterThan_ThenWeGetSettings()
        {
            var serializer = new StrategySettingsSerializer();
            var json = "{ \"Value\": 5 }";
            var strategy = serializer.Deserialize(StrategyNames.IsGreaterThan, json);

            strategy.Should().BeOfType<IsGreaterThanStrategy>();
            var expectedStrategy = strategy as IsGreaterThanStrategy;
            expectedStrategy.Settings.Value.Should().Be(5);
        }

        [Test]
        public void GivenASerializer_WhenDeserializingIsLowerThan_ThenWeGetSettings()
        {
            var serializer = new StrategySettingsSerializer();
            var json = "{ \"Value\": 5 }";
            var strategy = serializer.Deserialize(StrategyNames.IsLowerThan, json);

            strategy.Should().BeOfType<IsLowerThanStrategy>();
            var expectedStrategy = strategy as IsLowerThanStrategy;
            expectedStrategy.Settings.Value.Should().Be(5);
        }

        [Test]
        public void GivenASerializer_WhenDeserializingIsBefore_ThenWeGetSettings()
        {
            var clock = this.GivenClock().UtcNow;
            var dateInString = clock.ToString("O");
            var serializer = new StrategySettingsSerializer();
            var json = "{ \"Value\": \"" + dateInString + "\" }";
            var strategy = serializer.Deserialize(StrategyNames.IsBefore, json);

            strategy.Should().BeOfType<IsBeforeStrategy>();
            var expectedStrategy = strategy as IsBeforeStrategy;
            expectedStrategy.Settings.Value.Should().Be(clock);
        }

        [Test]
        public void GivenASerializer_WhenDeserializingIsAfter_ThenWeGetSettings()
        {
            var clock = this.GivenClock().UtcNow;
            var dateInString = clock.ToString("O");
            var serializer = new StrategySettingsSerializer();
            var json = "{ \"Value\": \"" + dateInString + "\" }";
            var strategy = serializer.Deserialize(StrategyNames.IsAfter, json);

            strategy.Should().BeOfType<IsAfterStrategy>();
            var expectedStrategy = strategy as IsAfterStrategy;
            expectedStrategy.Settings.Value.Should().Be(clock);
        }
    }
}