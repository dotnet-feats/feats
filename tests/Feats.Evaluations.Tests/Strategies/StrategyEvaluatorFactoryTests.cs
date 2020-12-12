using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Feats.Common.Tests;
using Feats.Common.Tests.Strategies;
using Feats.Common.Tests.Strategies.TestExtensions;
using Feats.Domain.Strategies;
using Feats.Evaluations.Strategies;
using FluentAssertions;
using NUnit.Framework;

namespace Feats.Evaluations.Tests.Strategies
{
    public class StrategyEvaluatorFactoryTests : TestBase
    {
        [Test]
        public async Task GivenIsOnStrategy_WhenEvaluating_ThenIGetOn()
        {
            var serializer = this.GivenIStrategySettingsSerializer();
            var strategy = new IsOnStrategy {
                Settings = new IsOnStrategySettings 
                {
                    IsOn = true
                }
            };

            await this
                .GivenEvaluatorFactory()
                .WhenEvaluating(StrategyNames.IsOn, serializer.Serialize(strategy.Settings))
                .ThenIGet(true);
        }
        
        [Test]
        public async Task GivenIsGreaterThanStrategy_WhenEvaluating_ThenIGetOn()
        {
            var serializer = this.GivenIStrategySettingsSerializer();
            var strategy = new IsGreaterThanStrategy() {
                Settings = new NumericalStrategySettings() 
                {
                    Value = 1
                }
            };

            await this
                .GivenEvaluatorFactory()
                .WhenEvaluating(
                    StrategyNames.IsGreaterThan, 
                    serializer.Serialize(strategy.Settings),
                    new Dictionary<string, string> { { StrategySettings.GreaterThan, "5" } })
                .ThenIGet(true);
        }
        
        [Test]
        public async Task GivenIsLowerThanStrategy_WhenEvaluating_ThenIGetOn()
        {
            var serializer = this.GivenIStrategySettingsSerializer();
            var strategy = new IsLowerThanStrategy() {
                Settings = new NumericalStrategySettings() 
                {
                    Value = 1
                }
            };

            await this
                .GivenEvaluatorFactory()
                .WhenEvaluating(
                    StrategyNames.IsLowerThan, 
                    serializer.Serialize(strategy.Settings),
                    new Dictionary<string, string> { { StrategySettings.LowerThan, "0" } })
                .ThenIGet(true);
        }

        [Test]
        public async Task GivenIsInListStrategy_WhenEvaluating_ThenIGetStrategyResult()
        {
            var serializer = this.GivenIStrategySettingsSerializer();
            var strategy = new IsInListStrategy {
                Settings = new IsInListStrategySettings 
                {
                    Items = new List<string> { "a", "😎🦝" }
                }
            };

            await this
                .GivenEvaluatorFactory()
                .WhenEvaluating(
                    StrategyNames.IsInList, 
                    serializer.Serialize(strategy.Settings),
                    new Dictionary<string, string> { { StrategySettings.List, "a" } })
                .ThenIGet(true);
        }

        [Test]
        public async Task GivenIsOffStrategy_WhenEvaluating_ThenIGetOff()
        {
            var serializer = new StrategySettingsSerializer();
            var strategy = new IsOnStrategy {
                Settings = new IsOnStrategySettings 
                {
                    IsOn = false
                }
            };

            await this
                .GivenEvaluatorFactory()
                .WhenEvaluating(StrategyNames.IsOn, serializer.Serialize(strategy.Settings))
                .ThenIGet(strategy);
        }
    }

    public static class StrategyEvaluatorFactoryTestsExtensions
    {
        public static IStrategyEvaluatorFactory GivenEvaluatorFactory(
            this StrategyEvaluatorFactoryTests tests)
        {
            return new StrategyEvaluatorFactory(
                tests.GivenIStrategySettingsSerializer(),
                new IsOnStrategyEvaluator(),
                new IsInListStrategyEvaluator(),
                new IsGreaterThanStrategyEvaluator(tests.GivenLogger<IsGreaterThanStrategyEvaluator>()),
                new IsLowerThanStrategyEvaluator(tests.GivenLogger<IsLowerThanStrategyEvaluator>())
            );
        }

        public static Func<Task<bool>> WhenEvaluating(
            this IStrategyEvaluatorFactory evaluatorFactory,
            string name, 
            string json,
            IDictionary<string,string> values = null)
        {
            return () => evaluatorFactory.IsOn(name, json, values);
        }

        public static  async Task ThenIGet(
            this Func<Task<bool>> evaluationFunc,
            bool expectedToBe)
        {
            var result = await evaluationFunc();
            result.Should().Be(expectedToBe);
        }
    }
}