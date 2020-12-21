using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Feats.Evaluation.Client.Tests
{
    public class IServiceCollectionExtensionTests
    {
        [Test]
        public void GivenAConfiguration_WhenAddingFeatureClient_ThenICanCreateMyClient()
        {
            var values = new Dictionary<string, string>()
            {
                {"feats:host", "something"},
                {"feats:request_timeout_in_seconds", "60"},
                {"feats:cache_timeout_in_seconds", "2"}
            };
            var services = new ServiceCollection();
            var configuration = new ConfigurationBuilder()
                .WithValues(values)
                .Build();
            services
                .AddFeatsEvaluationClient(configuration);

            var provider = services.BuildServiceProvider();

            using var client = provider.GetRequiredService<IFeatsEvaluationClient>();
            
            client.Should().NotBeNull();
        }
    }
}