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
    public class IsBeforeStrategyEvaluatorTests : TestBase
    {
        private readonly ISystemClock _clock;

        public IsBeforeStrategyEvaluatorTests()
        {
            this._clock = this.GivenClock();
        }

        [Test]
        public async Task GivenIntPointBeforeValue_WhenEvaluating_ThenIGetOn()
        {
            var strategy = new IsBeforeStrategy {
                Settings = new DateTimeOffsetStrategySettings 
                {
                    Value = this._clock.UtcNow
                }
            };
            var testedValue = this._clock.UtcNow.AddDays(-2).ToString("O");

            await this
                .GivenEvaluator()
                .WhenEvaluating(strategy, new Dictionary<string, string> { { StrategySettings.Before, testedValue } })
                .ThenIGet(true);
        }
        
        [Test]
        public async Task GivenNotBeforeStrategy_WhenEvaluating_ThenIGetOff()
        {
            var strategy = new IsBeforeStrategy {
                Settings = new DateTimeOffsetStrategySettings 
                {
                    Value = this._clock.UtcNow
                }
            };
            var testedValue = this._clock.UtcNow.AddDays(2).ToString("O");

            await this
                .GivenEvaluator()
                .WhenEvaluating(strategy, new Dictionary<string, string> { { StrategySettings.Before, testedValue } })
                .ThenIGet(false);
        }

        [Test]
        public async Task GivenMissingSettingInValues_WhenEvaluating_ThenIGetOff()
        {
            var strategy = new IsBeforeStrategy {
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
            var strategy = new IsBeforeStrategy {
                Settings = new DateTimeOffsetStrategySettings() 
                {
                    Value = this._clock.UtcNow
                }
            };

            await this
                .GivenEvaluator()
                .WhenEvaluating(strategy, new Dictionary<string, string> { { StrategySettings.Before, "a" } })
                .ThenIGet(false);
        }
    }

    public static class IsBeforeStrategyEvaluatorTestsExtensions
    {
        public static IsBeforeStrategyEvaluator GivenEvaluator(
            this IsBeforeStrategyEvaluatorTests tests)
        {
            return new IsBeforeStrategyEvaluator(tests.GivenLogger<IsBeforeStrategyEvaluator>());
        }

        public static Func<Task<bool>> WhenEvaluating(
            this IsBeforeStrategyEvaluator evaluator,
            IsBeforeStrategy strategy,
            IDictionary<string, string> values)
        {
            return () => evaluator.IsOn(strategy, values);
        }
    }
}