using System;
using System.Threading.Tasks;
using Feats.Common.Tests;
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
            var strategy = new IsOnStrategy {
                Settings = new IsOnStrategySettings 
                {
                    IsOn = true,
                },
            };

            await this
                .GivenEvaluatorFactory()
                .WhenEvaluating(strategy)
                .ThenIGet(strategy);
        }
    }

    public static class StrategyEvaluatorFactoryTestsExtensions
    {
        public static IStrategyEvaluatorFactory GivenEvaluatorFactory(
            this StrategyEvaluatorFactoryTests tests)
        {
            return new StrategyEvaluatorFactory(
                new IsOnStrategyEvaluator()
            );
        }

        public static Func<Task<bool>> WhenEvaluating(
            this IStrategyEvaluatorFactory evaluatorFactory,
            IsOnStrategy strategy)
        {
            return () => evaluatorFactory.IsOn(strategy);
        }
    }
}