using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Feats.Common.Tests;
using Feats.Domain.Strategies;
using Feats.Evaluations.Strategies;
using NUnit.Framework;

namespace Feats.Evaluations.Tests.Strategies
{
    public class IsGreaterThanStrategyEvaluatorTests : TestBase
    {
        [Test]
        public async Task GivenIntPointGreaterThanValue_WhenEvaluating_ThenIGetOn()
        {
            var strategy = new IsGreaterThanStrategy {
                Settings = new NumericalStrategySettings 
                {
                    Value = 1
                }
            };

            await this
                .GivenEvaluator()
                .WhenEvaluating(strategy, new Dictionary<string, string> { { StrategySettings.GreaterThan, "5" } })
                .ThenIGet(true);
        }
        
        [Test]
        public async Task GivenNegativeIntPointGreaterThanValue_WhenEvaluating_ThenIGetOn()
        {
            var strategy = new IsGreaterThanStrategy {
                Settings = new NumericalStrategySettings 
                {
                    Value = -500
                }
            };

            await this
                .GivenEvaluator()
                .WhenEvaluating(strategy, new Dictionary<string, string> { { StrategySettings.GreaterThan, "-5" } })
                .ThenIGet(true);
        }
        
        [Test]
        public async Task GivenFloatingPointGreaterThanValue_WhenEvaluating_ThenIGetOn()
        {
            var strategy = new IsGreaterThanStrategy {
                Settings = new NumericalStrategySettings 
                {
                    Value = 1
                }
            };

            await this
                .GivenEvaluator()
                .WhenEvaluating(strategy, new Dictionary<string, string> { { StrategySettings.GreaterThan, "1.1231231231" } })
                .ThenIGet(true);
        }
        
        [Test]
        public async Task GivenNegativeFloatingPointGreaterThanValue_WhenEvaluating_ThenIGetOn()
        {
            var strategy = new IsGreaterThanStrategy {
                Settings = new NumericalStrategySettings 
                {
                    Value = -5
                }
            };

            await this
                .GivenEvaluator()
                .WhenEvaluating(strategy, new Dictionary<string, string> { { StrategySettings.GreaterThan, "-1.1231231231" } })
                .ThenIGet(true);
        }
        
        [Test]
        public async Task GivenNotGreaterThanStrategy_WhenEvaluating_ThenIGetOff()
        {
            var strategy = new IsGreaterThanStrategy {
                Settings = new NumericalStrategySettings 
                {
                    Value = 100
                }
            };

            await this
                .GivenEvaluator()
                .WhenEvaluating(strategy, new Dictionary<string, string> { { StrategySettings.GreaterThan, "0000" } })
                .ThenIGet(false);
        }

        [Test]
        public async Task GivenMissingSettingInValues_WhenEvaluating_ThenIGetOff()
        {
            var strategy = new IsGreaterThanStrategy {
                Settings = new NumericalStrategySettings() 
                {
                    Value = 123.12345
                }
            };

            await this
                .GivenEvaluator()
                .WhenEvaluating(strategy, new Dictionary<string, string> { { "something", "0" } })
                .ThenIGet(false);
        }
        

        [Test]
        public async Task GivenInvalidNumericalValue_WhenEvaluating_ThenIGetOff()
        {
            var strategy = new IsGreaterThanStrategy {
                Settings = new NumericalStrategySettings() 
                {
                    Value = 123.12345
                }
            };

            await this
                .GivenEvaluator()
                .WhenEvaluating(strategy, new Dictionary<string, string> { { StrategySettings.GreaterThan, "a" } })
                .ThenIGet(false);
        }
    }

    public static class IsGreaterThanStrategyEvaluatorTestsExtensions
    {
        public static IsGreaterThanStrategyEvaluator GivenEvaluator(
            this IsGreaterThanStrategyEvaluatorTests tests)
        {
            return new IsGreaterThanStrategyEvaluator(tests.GivenLogger<IsGreaterThanStrategyEvaluator>());
        }

        public static Func<Task<bool>> WhenEvaluating(
            this IsGreaterThanStrategyEvaluator evaluator,
            IsGreaterThanStrategy strategy,
            IDictionary<string, string> values)
        {
            return () => evaluator.IsOn(strategy, values);
        }
    }
}