using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventStore.Client;
using Feats.Common.Tests;
using Feats.CQRS.Streams;
using Feats.Management.EventStoreSetups;
using Feats.Management.Features.Events;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace Feats.EventStoreS.Tests
{
    [TestFixture]
    [Category("Integration")]
    public class IEventStoreClientTests : TestBase
    {
        private IEventStoreConfiguration _configuration;

        private EventStoreClientFactory _factory;

        [SetUp]
        public void Up()
        {
            // for now i take for granted that you have launche docker compose :P
            // cause i'm lazy
            this._configuration = new EventStoreConfiguration(
                new ConfigurationBuilder().Build()
            );

            this._factory = new EventStoreClientFactory(this._configuration);
        }

        [Test]
        public async Task GivenEventStore_WhenReadingStream_ThenWeGetEvents()
        {
            var client = this._factory.Create();

            var results = client.ReadStreamAsync(
                new FeatureStream(), 
                EventStore.Client.Direction.Forwards,
                0);

            if (results == null)
            {
                Assert.Inconclusive("the current event store is empty, but we could connect");
                return;
            }

            await foreach(var e in results)
            {
                e.Should().NotBe(null);
            }
        }
        
        [Test]
        public async Task GivenEventStore_WhenPublishingToStream_ThenWeGetEventPublished()
        {
            var client = this._factory.Create();

            var data = new List<EventData> 
            {
                new FeatureCreatedEvent {
                    Name = "🦄",
                }.ToEventData(),
                new FeatureCreatedEvent {
                    Name = "bob",
                }.ToEventData(),
            };

            var results = await client.AppendToStreamAsync(
                new FeatureStream(),
                StreamState.Any,
                data);

            if (results == null)
            {
                Assert.Inconclusive("the current event store is empty, but we could connect");
                return;
            }

            results.NextExpectedStreamRevision.ToUInt64().Should().NotBe(ulong.MinValue);
        }
    }

    public static class IEventStoreClientTestsExtenstions
    {
        
    }
}
