using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Feats.Common.Tests;
using Feats.Domain.Strategies;
using Feats.Evaluations.Strategies;
using Microsoft.Extensions.Internal;
using NUnit.Framework;

namespace Feats.Evaluations.Tests.Strategies
{
    public class IsAfterStrategyEvaluatorTests : TestBase
    {
        private readonly ISystemClock _clock;

        public IsAfterStrategyEvaluatorTests()
        {
            this._clock = this.GivenClock();
        }

        [Test]
        public async Task GivenIntPointAfterValue_WhenEvaluating_ThenIGetOn()
        {
            var strategy = new IsAfterStrategy {
                Settings = new DateTimeOffsetStrategySettings 
                {
                    Value = this._clock.UtcNow
                }
            };
            var testedValue = this._clock.UtcNow.AddDays(2).ToString("O");

            await this
                .GivenEvaluator()
                .WhenEvaluating(strategy, new Dictionary<string, string> { { StrategySettings.After, testedValue } })
                .ThenIGet(true);
        }
        
        [Test]
        public async Task GivenNotAfterStrategy_WhenEvaluating_ThenIGetOff()
        {
            var strategy = new IsAfterStrategy {
                Settings = new DateTimeOffsetStrategySettings 
                {
                    Value = this._clock.UtcNow
                }
            };
            var testedValue = this._clock.UtcNow.AddDays(-2).ToString("O");

            await this
                .GivenEvaluator()
                .WhenEvaluating(strategy, new Dictionary<string, string> { { StrategySettings.After, testedValue } })
                .ThenIGet(false);
        }

        [Test]
        public async Task GivenMissingSettingInValues_WhenEvaluating_ThenIGetOff()
        {
            var strategy = new IsAfterStrategy {
                Settings = new DateTimeOffsetStrategySettings() 
                {
                    Value = this._clock.UtcNow
                }
            };

            await this
                .GivenEvaluator()
                .WhenEvaluating(strategy, new Dictionary<string, string> { { "something", "0" } })
                .ThenIGet(false);
        }
        

        [Test]
        public async Task GivenInvalidDateValue_WhenEvaluating_ThenIGetOff()
        {
            var strategy = new IsAfterStrategy {
                Settings = new DateTimeOffsetStrategySettings() 
                {
                    Value = this._clock.UtcNow
                }
            };

            await this
                .GivenEvaluator()
                .WhenEvaluating(strategy, new Dictionary<string, string> { { StrategySettings.After, "a" } })
                .ThenIGet(false);
        }
    }

    public static class IsAfterStrategyEvaluatorTestsExtensions
    {
        public static IsAfterStrategyEvaluator GivenEvaluator(
            this IsAfterStrategyEvaluatorTests tests)
        {
            return new IsAfterStrategyEvaluator(tests.GivenLogger<IsAfterStrategyEvaluator>());
        }

        public static Func<Task<bool>> WhenEvaluating(
            this IsAfterStrategyEvaluator evaluator,
            IsAfterStrategy strategy,
            IDictionary<string, string> values)
        {
            return () => evaluator.IsOn(strategy, values);
        }
    }
}