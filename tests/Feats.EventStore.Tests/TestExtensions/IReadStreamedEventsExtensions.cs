using System.Collections.Generic;
using System.Linq;
using Feats.Common.Tests;
using Feats.CQRS.Events;
using Feats.CQRS.Streams;
using Moq;

namespace Feats.EventStore.Tests.TestExtensions
{
    public static class IReadStreamedEventsExtensions
    {
        public static Mock<IReadStreamedEvents<TStream>> GivenIReadStreamedEvents<TStream>(this TestBase testClass)
            where TStream: IStream
        {
            return new Mock<IReadStreamedEvents<TStream>>();
        }

        public static Mock<IReadStreamedEvents<TStream>> WithEvents<TStream>(
            this Mock<IReadStreamedEvents<TStream>>  mock, 
            IEnumerable<IEvent> events)
            where TStream: IStream
        {
            mock.Setup(_ => _.Read())
                .Returns(events.ToAsyncEnumerable());

            return mock;
        }
    }
}