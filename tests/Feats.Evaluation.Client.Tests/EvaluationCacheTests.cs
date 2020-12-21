using System;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Extensions;
using Moq;
using NUnit.Framework;

namespace Feats.Evaluation.Client.Tests
{
    public class EvaluationCacheTests
    {
        [Test]
        public async Task GivenEmptyCache_WhenCheckingIsOn_ThenWeCallFactoryMethod()
        {
            var called = 0;
            var results = await this.GivenEvaluationCache()
                .isOn(new FeatureEvaluationRequest(), () =>
                {
                    called++;
                    return Task.FromResult(true);
                });
            called.Should().Be(1);
            results.Should().BeTrue();
        }
        
        [Test]
        public async Task GivenNonEmptyCache_WhenCheckingIsOn_ThenWeDontCallFActoryMethod()
        {
            var called = 0;
            var request = new FeatureEvaluationRequest
            {
                Name = "a",
                Path = "b"
            };
            var cache = this.GivenEvaluationCache();
            var results = await cache
                .isOn(request, () =>
                {
                    called++;
                    return Task.FromResult(true);
                });
            
            var resultsAgain = await cache
                .isOn(request, () =>
                {
                    called++;
                    return Task.FromResult(true);
                });
            
            called.Should().Be(1);
            results.Should().BeTrue();
            resultsAgain.Should().BeTrue();
        }
        
        
        [Test]
        public async Task GivenExpiredEntry_WhenCheckingIsOn_ThenWeRecallFactoryMethod()
        {
            var called = 0;
            var request = new FeatureEvaluationRequest
            {
                Name = "a",
                Path = "b"
            };
            var cache = this.GivenEvaluationCache();
            var results = await cache
                .isOn(request, () =>
                {
                    called++;
                    return Task.FromResult(true);
                });

            await Task.Delay(200.Milliseconds());
            var resultsAgain = await cache
                .isOn(request, () =>
                {
                    called++;
                    return Task.FromResult(true);
                });
            
            called.Should().Be(2);
            results.Should().BeTrue();
            resultsAgain.Should().BeTrue();
        }
    }

    internal static class EvaluationCacheTestsExtensions
    {
        internal static IEvaluationCache GivenEvaluationCache(
            this EvaluationCacheTests tests)
        {
            var mock = new Mock<IFeatsEvaluationConfiguration>();
            mock.Setup(_ => _.CacheTimeout).Returns(100.Milliseconds());
            
            return new EvaluationCache(mock.Object);
        }
    }
}