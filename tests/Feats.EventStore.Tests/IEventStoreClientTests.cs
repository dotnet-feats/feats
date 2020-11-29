using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using EventStore.Client;
using Feats.Common.Tests;
using Feats.CQRS.Events;
using Feats.CQRS.Streams;
using Feats.EventStore;
using FluentAssertions;
using NUnit.Framework;
using Microsoft.Extensions.Configuration;

namespace Feats.EventStoreS.Tests
{
    public class IEventStoreClientTests : TestBase
    {
        [Test]
        public async Task GivenEventStore_WhenReadingStream_ThenWeGetEvents()
        {
            var configuration = new EventStoreConfiguration(
                new ConfigurationBuilder().Build()
            );

            var factory = new EventStoreClientFactory(configuration);

            using var client = factory.Create();

            var results = client.ReadStreamAsync(
                new TestStream(), 
                Direction.Forwards,
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
            var configuration = new EventStoreConfiguration(
                new ConfigurationBuilder().Build()
            );

            var factory = new EventStoreClientFactory(configuration);

            using var client = factory.Create();

            var data = new List<EventData> 
            {
                new TestEvent {
                    Name = "🦄",
                }.ToEventData(),
                new TestEvent {
                    Name = "bob",
                }.ToEventData(),
            };

            var results = await client.AppendToStreamAsync(
                new TestStream(),
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

    public class TestStream : IStream
    {
        public string Name => "streams.tests.integration";
    }

    public class TestEvent : IEvent
    {
        public string Type => "test";

        public string Name { get; set; }
    }
    

    public static class FeatureCreatedEventExtensions
    {
        public static EventData ToEventData(this TestEvent featureCreatedEvent, JsonSerializerOptions settings = null)
        {
            var contentBytes = JsonSerializer.SerializeToUtf8Bytes(featureCreatedEvent, settings);
            return new EventData(
                eventId: Uuid.NewUuid(),
                type : featureCreatedEvent.Type,
                data: contentBytes
            );
        }
    }
}
