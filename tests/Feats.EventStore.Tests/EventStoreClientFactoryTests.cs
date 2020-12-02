using System;
using System.Collections.Generic;
using Feats.Common.Tests;
using Feats.EventStore;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace Feats.EventStoreS.Tests
{
    public class EventStoreClientFactoryTests : TestBase
    {
        [Test]
        public void GivenAFactory_WhenCreatingACLient_thenAClientIsInstanciated()
        {
            var configuration = new EventStoreConfiguration(
                new ConfigurationBuilder().Build()
            );

            var factory = new EventStoreClientFactory(configuration);

            var client = factory.Create();

            client.Should().NotBeNull();
        }
        
        [Test]
        public void GivenAFactory_WhenCreatingAClusterModeClient_thenAClientIsInstanciated()
        {
            var values = new Dictionary<string, string> {
                { "feats:eventstore:hostname", "host" },
                { "feats:eventstore:protocol", "https" },
                { "feats:eventstore:username", "user" },
                { "feats:eventstore:password", "pwd" },
                { "feats:eventstore:iscluster", "true" },
            };

            var configuration = new EventStoreConfiguration(
                new ConfigurationBuilder()
                    .AddInMemoryCollection(values)
                    .Build()
            );

            var factory = new EventStoreClientFactory(configuration);

            var client = factory.Create();

            client.Should().NotBeNull();
        }
    }
}