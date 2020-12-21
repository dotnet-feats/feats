using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Feats.Common.Tests;
using Feats.CQRS.Events;
using Feats.CQRS.Streams;
using Feats.Domain.Events;
using Feats.EventStore.Tests.TestExtensions;
using FluentAssertions;
using NUnit.Framework;

namespace Feats.EventStore.Tests.Events
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
            var eventOne = new PathCreatedEvent
            {
                Path = "🦄",
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
                .ThenWeReturnEmptyList();
        }

        [Test]
        public async Task GivenEventsInStream_WhenReading_ThenWeReturnEventList()
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
        internal static FeatureStreamEventsReader GivenFeatureStreamEventsReader(
            this FeatureStreamEventsReaderTests tests,
            IEventStoreClient client)
        {
            return new FeatureStreamEventsReader(
                tests.GivenLogger<FeatureStreamEventsReader>(), 
                client);
        }

        internal static Func<IAsyncEnumerable<IEvent>> WhenReading(
            this FeatureStreamEventsReader reader)
        {
            return () => reader.Read();
        }
    }
}
