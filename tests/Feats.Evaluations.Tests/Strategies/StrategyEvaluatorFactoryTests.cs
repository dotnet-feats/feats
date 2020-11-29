using System;
using System.Threading.Tasks;
using Feats.Common.Tests;
using Feats.Common.Tests.Strategies;
using Feats.Domain.Strategies;
using Feats.Evaluations.Strategies;
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
                    IsOn = true,
                },
            };

            await this
                .GivenEvaluatorFactory()
                .WhenEvaluating(StrategyNames.IsOn, serializer.Serialize(strategy.Settings))
                .ThenIGet(strategy);
        }
        
        [Test]
        public async Task GivenIsOffStrategy_WhenEvaluating_ThenIGetOn()
        {
            var serializer = new StrategySettingsSerializer();
            var strategy = new IsOnStrategy {
                Settings = new IsOnStrategySettings 
                {
                    IsOn = false,
                },
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
                new IsOnStrategyEvaluator()
            );
        }

        public static Func<Task<bool>> WhenEvaluating(
            this IStrategyEvaluatorFactory evaluatorFactory,
            string name, 
            string json)
        {
            return () => evaluatorFactory.IsOn(name, json);
        }
    }
}