using System;
using System.Threading.Tasks;
using Feats.Common.Tests;
using Feats.Domain.Strategies;
using Feats.Evaluations.Strategies;
using FluentAssertions;
using NUnit.Framework;

namespace Feats.Evaluations.Tests.Strategies
{
    public class IsOnStrategyEvaluatorTests : TestBase
    {
        [Test]
        public async Task GivenOnStrategy_WhenEvaluating_ThenIGetOn()
        {
            var strategy = new IsOnStrategy {
                Settings = new IsOnStrategySettings 
                {
                    IsOn = true,
                },
            };

            await this
                .GivenEvaluator()
                .WhenEvaluating(strategy)
                .ThenIGet(strategy);
        }

        [Test]
        public async Task GivenOffStrategy_WhenEvaluating_ThenIGetOff()
        {
            var strategy = new IsOnStrategy {
                Settings = new IsOnStrategySettings 
                {
                    IsOn = false,
                },
            };

            await this
                .GivenEvaluator()
                .WhenEvaluating(strategy)
                .ThenIGet(strategy);
        }
    }

    public static class IsOnStrategyEvaluatorTestsExtensions
    {
        public static IsOnStrategyEvaluator GivenEvaluator(this IsOnStrategyEvaluatorTests tests)
        {
            return new IsOnStrategyEvaluator();
        }

        public static Func<Task<bool>> WhenEvaluating(
            this IsOnStrategyEvaluator evaluator,
            IsOnStrategy strategy)
        {
            return () => evaluator.IsOn(strategy);
        }        

        public static  async Task ThenIGet(
            this Func<Task<bool>> evaluationFunc,
            IsOnStrategy strategy)
        {
            var result = await evaluationFunc();
            result.Should().Be(strategy.Settings.IsOn);
        }
    }
}