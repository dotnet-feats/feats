using System;
using System.Collections.Generic;
using Feats.Common.Tests;
using Feats.EventStore;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace Feats.EventStoreS.Tests
{
    public class EventStoreConfigurationTests : TestBase
    {
        [Test]
        public void GivenNullConfiguration_WhenBuilding_ThenWeThrow()
        {
            var func = new Func<IEventStoreConfiguration>(() => new EventStoreConfiguration(null));

            func.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void GivenEmptyConfiguration_WhenBuilding_ThenWeGetDefaults()
        {
            var configuration = new EventStoreConfiguration(
                new ConfigurationBuilder().Build()
            );

            configuration.HostName.Should().Be("localhost");
            configuration.Protocol.Should().Be("http");
            configuration.Username.Should().Be("admin");
            configuration.Password.Should().Be("changeit");
            configuration.IsClusterModeEnabled.Should().Be(false);
        }
        
        [Test]
        public void GivenValidtConfiguration_WhenBuilding_ThenWeGetTheValues()
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

            configuration.HostName.Should().Be("host");
            configuration.Protocol.Should().Be("https");
            configuration.Username.Should().Be("user");
            configuration.Password.Should().Be("pwd");
            configuration.IsClusterModeEnabled.Should().Be(true);
        }
    }
}