using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using EventStore.Client;
using Feats.Common.Tests;
using Feats.CQRS.Streams;
using Feats.Management.EventStoreSetups;
using Moq;

namespace Feats.Management.Tests.EventStoreSetups.TestExtensions
{
    public static class IEventStoreClientExtensions 
    {
        public static Mock<IEventStoreClient> GivenIEventStoreClient(this TestBase testClass)
        {
            return new Mock<IEventStoreClient>();
        }

        public static Mock<IEventStoreClient> WithAppendToStreamAsync(
            this Mock<IEventStoreClient> mock,
            IStream stream)
        {
            mock.Setup(_ => _.AppendToStreamAsync(
                It.Is<IStream>(s => s.Name.Equals(stream.Name, StringComparison.InvariantCultureIgnoreCase)), 
                It.IsAny<StreamState>(),
                It.IsAny<IEnumerable<EventData>>(),
                It.IsAny<Action<EventStoreClientOperationOptions>?>(),
                It.IsAny<UserCredentials?>(),
                It.IsAny<CancellationToken>()));

            return mock;
        }

        public static Mock<IEventStoreClient> WithReadStreamAsync(
            this Mock<IEventStoreClient> mock,
            IStream stream,
            IDictionary<string, byte[]> contents)
        {
            var records = contents.Select((b, i) => {
            var rec = new EventRecord(
                stream.Name,
                Uuid.NewUuid(),
                StreamPosition.Start,
                Position.Start,
                new Dictionary<string, string>() 
                {
                    { "type", b.Key},
                    { "created", DateTime.UtcNow.Ticks.ToString()},
                    { "content-type", "application/json"},
                },
                new ReadOnlyMemory<byte>(b.Value),
                null);

            return new ResolvedEvent(rec, null, null);
           }).ToAsyncEnumerable();

            mock.Setup(_ => _.ReadStreamAsync(
                It.Is<IStream>(s => s.Name.Equals(stream.Name, StringComparison.InvariantCultureIgnoreCase)), 
                Direction.Forwards,
                StreamPosition.Start,
                It.IsAny<long>(),
                It.IsAny<Action<EventStoreClientOperationOptions>?>(),
                It.IsAny<bool>(),
                It.IsAny<UserCredentials?>(),
                It.IsAny<CancellationToken>()))
            .Returns(records);

            return mock;
        }
    }
}