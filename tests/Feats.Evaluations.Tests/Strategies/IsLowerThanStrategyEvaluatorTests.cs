using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Feats.Common.Tests;
using Feats.Domain.Strategies;
using Feats.Evaluations.Strategies;
using NUnit.Framework;

namespace Feats.Evaluations.Tests.Strategies
{
    public class IsLowerThanStrategyEvaluatorTests : TestBase
    {
        [Test]
        public async Task GivenIntPointLowerThanValue_WhenEvaluating_ThenIGetOn()
        {
            var strategy = new IsLowerThanStrategy {
                Settings = new NumericalStrategySettings 
                {
                    Value = 1000
                }
            };

            await this
                .GivenEvaluator()
                .WhenEvaluating(strategy, new Dictionary<string, string> { { StrategySettings.LowerThan, "5" } })
                .ThenIGet(true);
        }
        
        [Test]
        public async Task GivenNegativeIntPointLowerThanValue_WhenEvaluating_ThenIGetOn()
        {
            var strategy = new IsLowerThanStrategy {
                Settings = new NumericalStrategySettings 
                {
                    Value = -1
                }
            };

            await this
                .GivenEvaluator()
                .WhenEvaluating(strategy, new Dictionary<string, string> { { StrategySettings.LowerThan, "-5" } })
                .ThenIGet(true);
        }
        
        [Test]
        public async Task GivenFloatingPointLowerThanValue_WhenEvaluating_ThenIGetOn()
        {
            var strategy = new IsLowerThanStrategy {
                Settings = new NumericalStrategySettings 
                {
                    Value = 3.33
                }
            };

            await this
                .GivenEvaluator()
                .WhenEvaluating(strategy, new Dictionary<string, string> { { StrategySettings.LowerThan, "1.1231231231" } })
                .ThenIGet(true);
        }
        
        [Test]
        public async Task GivenNegativeFloatingPointLowerThanValue_WhenEvaluating_ThenIGetOn()
        {
            var strategy = new IsLowerThanStrategy {
                Settings = new NumericalStrategySettings 
                {
                    Value = -5
                }
            };

            await this
                .GivenEvaluator()
                .WhenEvaluating(strategy, new Dictionary<string, string> { { StrategySettings.LowerThan, "-19.1231231231" } })
                .ThenIGet(true);
        }
        
        [Test]
        public async Task GivenNotLowerThanStrategy_WhenEvaluating_ThenIGetOff()
        {
            var strategy = new IsLowerThanStrategy {
                Settings = new NumericalStrategySettings 
                {
                    Value = 2
                }
            };

            await this
                .GivenEvaluator()
                .WhenEvaluating(strategy, new Dictionary<string, string> { { StrategySettings.LowerThan, "100" } })
                .ThenIGet(false);
        }

        [Test]
        public async Task GivenMissingSettingInValues_WhenEvaluating_ThenIGetOff()
        {
            var strategy = new IsLowerThanStrategy {
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
            var strategy = new IsLowerThanStrategy {
                Settings = new NumericalStrategySettings() 
                {
                    Value = 123.12345
                }
            };

            await this
                .GivenEvaluator()
                .WhenEvaluating(strategy, new Dictionary<string, string> { { StrategySettings.LowerThan, "a" } })
                .ThenIGet(false);
        }
    }

    public static class IsLowerThanStrategyEvaluatorTestsExtensions
    {
        public static IsLowerThanStrategyEvaluator GivenEvaluator(
            this IsLowerThanStrategyEvaluatorTests tests)
        {
            return new IsLowerThanStrategyEvaluator(tests.GivenLogger<IsLowerThanStrategyEvaluator>());
        }

        public static Func<Task<bool>> WhenEvaluating(
            this IsLowerThanStrategyEvaluator evaluator,
            IsLowerThanStrategy strategy,
            IDictionary<string, string> values)
        {
            return () => evaluator.IsOn(strategy, values);
        }
    }
}