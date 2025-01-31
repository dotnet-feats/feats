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
    public class PathStreamEventsReaderTests : TestBase
    {
        private readonly IStream _stream = new PathStream();

        [Test]
        public async Task GivenNoEventsInStream_WhenReading_ThenWeReturnEmptyList()
        {
            var results = new Dictionary<string, byte[]>();

            var client = this.GivenIEventStoreClient()
                .WithReadStreamAsync(this._stream, results);

            await PathStreamEventsReaderTestsExtenstions.ThenWeReturnEmptyList(this
                    .GivenPathStreamEventsReader(client.Object)
                    .WhenReading());
        }

        [Test]
        public async Task GivenUnsupportedEventInStream_WhenReading_ThenWeReturnEmptyList()
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

            await PathStreamEventsReaderTestsExtenstions.ThenWeReturnEmptyList(this
                    .GivenPathStreamEventsReader(client.Object)
                    .WhenReading());
        }

        [Test]
        public async Task GivenEventsInStream_Whenreading_ThenWeReturnEventList()
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

            await PathStreamEventsReaderTestsExtenstions.ThenWeReturnEventList(this
                    .GivenPathStreamEventsReader(client.Object)
                    .WhenReading(), new List<IEvent> { eventOne });
        }
    }

    public static class PathStreamEventsReaderTestsExtenstions
    {
        internal static PathStreamEventsReader GivenPathStreamEventsReader(
            this PathStreamEventsReaderTests tests,
            IEventStoreClient client)
        {
            return new PathStreamEventsReader(
                tests.GivenLogger<PathStreamEventsReader>(), 
                client);
        }

        internal static Func<IAsyncEnumerable<IEvent>> WhenReading(
            this PathStreamEventsReader reader)
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
