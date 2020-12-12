using System;
using System.Threading.Tasks;
using Feats.Common.Tests;
using Feats.CQRS.Events;
using Feats.CQRS.Streams;
using Feats.EventStore.Aggregates;

namespace Feats.EventStore.Tests.Aggregates
{
    public abstract class PathsAggregateTests : TestBase
    {
    }

    public static class PathsAggregateTestsExtensions
    {
        public static IPathsAggregate GivenAggregate(
            this PathsAggregateTests tests,
            IReadStreamedEvents<PathStream> reader,
            IEventStoreClient client)
        {
            return new PathsAggregate(
                tests.GivenLogger<PathsAggregate>(),
                reader,
                client);
        }

        public static async Task<IPathsAggregate> WithLoad(this IPathsAggregate aggregate)
        {
            await aggregate.Load();

            return aggregate;
        }

        public static Func<Task> WhenPublishing(this IPathsAggregate aggregate, IEvent e)
        {
            return () => aggregate.Publish(e);
        }
    }
}