using System;
using System.Collections.Generic;
using FluentAssertions;
using FluentAssertions.Extensions;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace Feats.Evaluation.Client.Tests
{
    public class FeatsEvaluationConfigurationTests
    {
        [Test]
        public void GivenAConfiguration_WhenMissingFeats_ThenWeThrow()
        {
            var values = new Dictionary<string, string>();
            this.GivenBuilder()
                .WithValues(values)
                .WhenBuilding()
                .Should().Throw<ArgumentNullException>();
        }
        
        [Test]
        public void GivenAConfiguration_WhenMissingHost_ThenWeThrow()
        {
            var values = new Dictionary<string, string>()
            {
                {"feats", "something"}
            };
            
            this.GivenBuilder()
                .WithValues(values)
                .WhenBuilding()
                .Should().Throw<ArgumentNullException>();
        }
        
        [Test]
        public void GivenAConfiguration_WhenMissingTimeouts_ThenWeUseDefaults()
        {
            var builder = new UriBuilder( "localhost");
            var values = new Dictionary<string, string>()
            {
                {"feats:host", "localhost"}
            };
            
            var result = this.GivenBuilder()
                .WithValues(values)
                .WhenBuilding()();
            result.Host.Should().Be(builder.Uri);
            result.RequestTimeout.Should().Be(5.Minutes());
            result.CacheTimeout.Should().Be(30.Seconds());
        }
        
        [Test]
        public void GivenAConfiguration_WhenBuilding_ThenWeUseConfigurationSettings()
        {
            var builder = new UriBuilder("something");
            var values = new Dictionary<string, string>()
            {
                {"feats:host", "something"},
                {"feats:request_timeout_in_seconds", "60"},
                {"feats:cache_timeout_in_seconds", "2"}
            };
            var result = this.GivenBuilder()
                .WithValues(values)
                .WhenBuilding()();
            result.Host.Should().Be(builder.Uri);
            result.RequestTimeout.Should().Be(1.Minutes());
            result.CacheTimeout.Should().Be(2.Seconds());
        }
        
        [Test]
        public void GivenAConfigurationWithFullUri_WhenBuilding_ThenWeUseConfigurationSettings()
        {
            var builder = new UriBuilder("https://something.dev:9999/");
            var values = new Dictionary<string, string>()
            {
                {"feats:host", "https://something.dev:9999/"},
                {"feats:request_timeout_in_seconds", "60"},
                {"feats:cache_timeout_in_seconds", "2"}
            };
            var result = this.GivenBuilder()
                .WithValues(values)
                .WhenBuilding()();
            result.Host.Should().Be(builder.Uri);
            result.RequestTimeout.Should().Be(1.Minutes());
            result.CacheTimeout.Should().Be(2.Seconds());
        }
    }

    internal static class FeatsEvaluationConfigurationTestsExtensions
    {
        internal static IConfigurationBuilder GivenBuilder(this FeatsEvaluationConfigurationTests tests)
        {
            return new ConfigurationBuilder();
        }

        internal static IConfigurationBuilder WithValues(
            this IConfigurationBuilder builder,
            IDictionary<string, string> values)
        {
            builder.AddInMemoryCollection(values);
            
            return builder;
        }

        internal static Func<IFeatsEvaluationConfiguration> WhenBuilding(this IConfigurationBuilder builder)
        {
            return() => new FeatsEvaluationConfiguration(builder.Build());
        }
    }
}