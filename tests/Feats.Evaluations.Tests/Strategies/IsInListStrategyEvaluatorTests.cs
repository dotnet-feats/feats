using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Feats.Common.Tests;
using Feats.Domain.Strategies;
using Feats.Evaluations.Strategies;
using NUnit.Framework;

namespace Feats.Evaluations.Tests.Strategies
{
    public class IsInListStrategyEvaluatorTests : TestBase
    {
        [Test]
        public async Task GivenInLIstStrategy_WhenEvaluating_ThenIGetOn()
        {
            var strategy = new IsInListStrategy {
                Settings = new IsInListStrategySettings 
                {
                    Items = new List<string> { "👌", "message for ou sir" }
                }
            };

            await this
                .GivenEvaluator()
                .WhenEvaluating(strategy, new Dictionary<string, string> { { StrategySettings.List, "👌" } })
                .ThenIGet(true);
        }
        
        [Test]
        public async Task GivenNotInListStrategy_WhenEvaluating_ThenIGetOff()
        {
            var strategy = new IsInListStrategy {
                Settings = new IsInListStrategySettings 
                {
                    Items = new List<string> { "👌", "message for ou sir" }
                }
            };

            await this
                .GivenEvaluator()
                .WhenEvaluating(strategy, new Dictionary<string, string> { { StrategySettings.List, "roger" } })
                .ThenIGet(false);
        }

        [Test]
        public async Task GivenMissingSettingInValues_WhenEvaluating_ThenIGetOff()
        {
            var strategy = new IsInListStrategy {
                Settings = new IsInListStrategySettings 
                {
                    Items = new List<string> { "👌", "message for ou sir" }
                }
            };

            await this
                .GivenEvaluator()
                .WhenEvaluating(strategy, new Dictionary<string, string> { { "something", "0" } })
                .ThenIGet(false);
        }
    }

    public static class IsInListStrategyEvaluatorTestsExtensions
    {
        public static IsInListStrategyEvaluator GivenEvaluator(
            this IsInListStrategyEvaluatorTests tests)
        {
            return new IsInListStrategyEvaluator();
        }

        public static Func<Task<bool>> WhenEvaluating(
            this IsInListStrategyEvaluator evaluator,
            IsInListStrategy strategy,
            IDictionary<string, string> values)
        {
            return () => evaluator.IsOn(strategy, values);
        }
    }
}