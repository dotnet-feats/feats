using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using FluentAssertions;
using NUnit.Framework;

namespace Feats.Evaluation.Client.Tests
{
    public class FeatureEvaluationRequestTests
    {
        [Test]
        public void GivenNoStrategiesToInject_WhenTransformingToHttpRequestMessage_ThenNOAdditionalHeadersAreFound()
        {
            var httpRequestMessage = this.GivenRequest()
                .ToHttpRequestMessage(UrlEncoder.Default);
            httpRequestMessage.Should().NotBeNull();
            httpRequestMessage.RequestUri.Should().NotBeNull();
            httpRequestMessage.RequestUri.ToString().Should().Contain("features?");
            httpRequestMessage.RequestUri.ToString().Should().Contain("path=");
            httpRequestMessage.RequestUri.ToString().Should().Contain("name=");
            httpRequestMessage.Headers.Should().BeEmpty();
        }
        
        [Test]
        public void GivenUrlSpecialCharacters_WhenTransformingToHttpRequestMessage_ThenQueryParametersAreEncoded()
        {
            var httpRequestMessage = this.GivenRequest("cat is a demon!", "where/can i @run")
                .ToHttpRequestMessage(UrlEncoder.Default);
            httpRequestMessage.Should().NotBeNull();
            httpRequestMessage.RequestUri.Should().NotBeNull();
            httpRequestMessage.RequestUri.ToString().Should().Contain("features?");
            httpRequestMessage.RequestUri.ToString().Should().Contain("path=where%252Fcan%2520i%2520%40run");
            httpRequestMessage.RequestUri.ToString().Should().Contain("name=cat%2520is%2520a%2520demon!");
            httpRequestMessage.Headers.Should().BeEmpty();
        }
        
        [Test]
        public void GivenStrategiesToInject_WhenTransformingToHttpRequestMessage_ThenAdditionalHeadersAreFound()
        {
            var httpRequestMessage = this.GivenRequest()
                .WithStrategies("a", "b")
                .ToHttpRequestMessage(UrlEncoder.Default);
            
            httpRequestMessage.Should().NotBeNull();
            httpRequestMessage.RequestUri.Should().NotBeNull();
            httpRequestMessage.RequestUri.ToString().Should().Contain("features?");
            httpRequestMessage.RequestUri.ToString().Should().Contain("path=");
            httpRequestMessage.RequestUri.ToString().Should().Contain("name=");
            httpRequestMessage.Headers.Should().NotBeEmpty();
            httpRequestMessage.Headers.GetValues("a").Should().BeEquivalentTo(new List<string> { "a" });
            httpRequestMessage.Headers.GetValues("b").Should().BeEquivalentTo(new List<string> { "b" });
        }
        
        [Test]
        public void GivenStrategies_WhenCalculatingCacheKey_ThenTwoIdenticalRequestsHaveSameKey()
        {
            var strategyA = this.GivenRequest()
                .WithStrategies("a", "#$%^  sfsfa")
                .GetCacheKey();
            
            var strategyB = this.GivenRequest()
                .WithStrategies("#$%^  sfsfa", "a")
                .GetCacheKey();
            strategyA.Should().Be(strategyB);
        }
        
        [Test]
        public void GivenStrategiesWithSpecialChars_WhenCalculatingCacheKey_ThenTwoIdenticalRequestsHaveSameKey()
        {
            var strategyA = this.GivenRequest()
                .WithStrategies("a", "🦝🦝🦝🦝🦝🦝🦝")
                .GetCacheKey();
            
            var strategyB = this.GivenRequest()
                .WithStrategies("🦝🦝🦝🦝🦝🦝🦝", "a")
                .GetCacheKey();
            strategyA.Should().Be(strategyB);
        }

        [Test]
        public void GivenNoStrategies_WhenCalculatingCacheKey_ThenTwoIdenticalRequestsHaveSameKey()
        {
            var strategyA = this.GivenRequest()
                .GetCacheKey();
            
            var strategyB = this.GivenRequest()
                .GetCacheKey();
            strategyA.Should().Be(strategyB);
        }
        
        [Test]
        public void GivenDifferentStrategies_WhenCalculatingCacheKey_ThenTheyDontShareTheCacheKey()
        {
            var strategyA = this.GivenRequest()
                .WithStrategies("a", "c")
                .GetCacheKey();
            
            var strategyB = this.GivenRequest()
                .WithStrategies("a", "b")
                .GetCacheKey();
            strategyA.Should().NotBe(strategyB);
        }
        
        [Test]
        public void GivenDifferentNames_WhenCalculatingCacheKey_ThenTheyDontShareTheCacheKey()
        {
            var strategyA = this.GivenRequest(name : "nope")
                .WithStrategies("a", "b")
                .GetCacheKey();
            
            var strategyB = this.GivenRequest()
                .WithStrategies("a", "b")
                .GetCacheKey();
            
            strategyA.Should().NotBe(strategyB);
        }
        
        [Test]
        public void GivenDifferentPaths_WhenCalculatingCacheKey_ThenTheyDontShareTheCacheKey()
        {
            var strategyA = this.GivenRequest(path : "nope")
                .WithStrategies("a", "#$%^  sfsfa")
                .GetCacheKey();
            
            var strategyB = this.GivenRequest()
                .WithStrategies("a", "#$%^  sfsfa")
                .GetCacheKey();
            
            strategyA.Should().NotBe(strategyB);
        }
    }

    internal static class FeatureEvaluationRequestTestsExtensions
    {
        internal static IFeatureEvaluationRequest GivenRequest(
            this FeatureEvaluationRequestTests tests,
            string name = "name",
            string path = "path")
        {
            return new FeatureEvaluationRequest
            {
                Name = name,
                Path = path,
                Strategies = new Dictionary<string, string>()
            };
        }
        internal static IFeatureEvaluationRequest WithStrategies(
            this IFeatureEvaluationRequest request,
            params string[] strategies)
        {
            return new FeatureEvaluationRequest
            {
                Name = request.Name,
                Path = request.Path,
                Strategies = strategies.Select(_ => new KeyValuePair<string, string>(_, _))
            };
        }
    }
}