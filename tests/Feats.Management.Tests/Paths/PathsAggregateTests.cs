using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using EventStore.Client;
using Feats.Common.Tests;
using Feats.CQRS.Events;
using Feats.CQRS.Streams;
using Feats.EventStore;
using Feats.Management.Features.Events;
using Feats.Management.Paths;
using Moq;

namespace Feats.Management.Tests.Paths
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
        
        public static async Task ThenWePublish(
            this Func<Task> funk,
            Mock<IEventStoreClient> mockedClient,
            PathCreatedEvent e)
        {
            await funk();

            mockedClient.Verify(
                _ => _.AppendToStreamAsync(
                    It.IsAny<PathStream>(),
                    It.IsAny<StreamState>(),
                    It.Is<IEnumerable<EventData>>(items => 
                        items.All(ed =>
                            ed.Type.Equals(EventTypes.PathCreated) && 
                            JsonSerializer.Deserialize<PathCreatedEvent>(ed.Data.ToArray(), null).Path.Equals(e.Path, StringComparison.InvariantCultureIgnoreCase)
                        )),
                    It.IsAny<Action<EventStoreClientOperationOptions>?>(),
                    It.IsAny<UserCredentials?>(),
                    It.IsAny<CancellationToken>()),
                Times.Once());
        }
    }
}