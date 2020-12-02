using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Feats.Common.Tests;
using Feats.CQRS.Events;
using Feats.CQRS.Streams;
using Feats.Domain.Events;
using Feats.EventStore;
using Feats.Evaluations.Tests.EventStoreSetups.TestExtensions;
using FluentAssertions;
using NUnit.Framework;
using Feats.Evaluations.Tests.Features;
using Feats.Evaluations.Features.Events;

namespace Feats.Evaluations.Features.Tests
{
    public class FeatureStreamEventsReaderTests : TestBase
    {
        private readonly IStream _stream = new FeatureStream();

        [Test]
        public async Task GivenNoEventsInStream_WhenReading_ThenWeReturnEmptyList()
        {
            var results = new Dictionary<string, byte[]>();

            var client = this.GivenIEventStoreClient()
                .WithReadStreamAsync(this._stream, results);

            await this
                .GivenFeatureStreamEventsReader(client.Object)
                .WhenReading()
                .ThenWeReturnEmptyList();
        }

        [Test]
        public async Task GivenUnsupportedEventInStream_WhenReading_ThenWeReturnEmptyList()
        {
            var eventOne = new NinjaEvent();

            var results = new Dictionary<string, byte[]>
            {
                { eventOne.Type, JsonSerializer.SerializeToUtf8Bytes(eventOne) },
            };

            var client = this.GivenIEventStoreClient()
                .WithReadStreamAsync(this._stream, results);

            await this
                .GivenFeatureStreamEventsReader(client.Object)
                .WhenReading()
                .ThenWeReturnEmptyList();
        }

        [Test]
        public async Task GivenEventsInStream_Whenreading_ThenWeReturnEventList()
        {
            var eventOne = new FeatureCreatedEvent
            {
                Name = "🦄",
            };

            var results = new Dictionary<string, byte[]>
            {
                { eventOne.Type, JsonSerializer.SerializeToUtf8Bytes(eventOne) },
            };

            var client = this.GivenIEventStoreClient()
                .WithReadStreamAsync(this._stream, results);

            await this
                .GivenFeatureStreamEventsReader(client.Object)
                .WhenReading()
                .ThenWeReturnEventList(new List<IEvent> { eventOne });
        }
    }

    public static class FeatureStreamEventsReaderTestsExtenstions
    {
        public static FeatureStreamEventsReader GivenFeatureStreamEventsReader(
            this FeatureStreamEventsReaderTests tests,
            IEventStoreClient client)
        {
            return new FeatureStreamEventsReader(
                tests.GivenLogger<FeatureStreamEventsReader>(), 
                client);
        }

        public static Func<IAsyncEnumerable<IEvent>> WhenReading(
            this FeatureStreamEventsReader reader)
        {
            return () => reader.Read();
        }

        public static async Task ThenWeReturnEventList(
            this Func<IAsyncEnumerable<IEvent>> resultsFunc,
            IEnumerable<IEvent> events)
        {
            var results = resultsFunc();
            var list = new List<IEvent>();
            await foreach(var e in results)
            {
                list.Add(e);
            }

            list.Should().BeEquivalentTo(events);
        }

        public static async Task ThenWeReturnEmptyList(
            this Func<IAsyncEnumerable<IEvent>> resultsFunc)
        {
            var results = resultsFunc();
            results.Should().NotBe(null);
            
            var list = new List<IEvent>();
            await foreach(var e in results)
            {
                list.Add(e);
            }

            list.Should().BeEmpty();
        }
    }
}
