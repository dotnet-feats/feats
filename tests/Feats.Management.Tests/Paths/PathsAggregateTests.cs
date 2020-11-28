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
using Feats.Management.Features.Exceptions;
using Feats.Management.Paths;
using Feats.Management.Tests.EventStoreSetups.TestExtensions;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Feats.Management.Tests.Paths
{
    public class PathsAggregateTests : TestBase
    {
        private readonly IStream _pathStream = new PathStream();

        [Test]
        public async Task GivenIdenticalPathAndFeature_WhenPublishingPathCreatedEvent_ThenWeTrowPathAndFeatureAlreadyExistsException()
        {
            var createdAlready = new PathCreatedEvent {
                FeatureAdded = "bob",
                Path = "let/me/show/you",
            };

            var client = this.GivenIEventStoreClient()
                .WithAppendToStreamAsync(this._pathStream);

            var reader = this.GivenIReadStreamedEvents<PathStream>()
                .WithEvents(new List<IEvent> { createdAlready });

            var aggregate = await this
                .GivenAggregate(reader.Object, client.Object)
                .WithLoad();

            await aggregate
                .WhenPublishing(createdAlready)
                .ThenExceptionIsThrown<PathAndFeatureAlreadyExistsException>();
        }

        [Test]
        public async Task GivenNoPreviousPaths_WhenPublishingPathCreatedEvent_ThenWePublish()
        {
            var client = this.GivenIEventStoreClient()
                .WithAppendToStreamAsync(this._pathStream);

            var reader = this.GivenIReadStreamedEvents<PathStream>()
                .WithEvents(Enumerable.Empty<IEvent>());

            var created = new PathCreatedEvent {
                FeatureAdded = "bob",
                Path = "let/me/show/you",
            };

            var aggregate = await this
                .GivenAggregate(reader.Object, client.Object)
                .WithLoad();

            await aggregate
                .WhenPublishing(created)
                .ThenWePublish(client, created);

            aggregate.Paths.Select(_ => _.Name).Should().BeEquivalentTo(new List<string> { created.Path });
        }
        
        [Test]
        public async Task GivenNotConflictingPaths_WhenPublishingPathCreatedEvent_ThenWePublishTheFeature()
        {
            var createdAlready = new PathCreatedEvent {
                FeatureAdded = "bob",
                Path = "let/me/show/you",
            };
            
            var client = this.GivenIEventStoreClient()
                .WithAppendToStreamAsync(this._pathStream);

            var reader = this.GivenIReadStreamedEvents<PathStream>()
                .WithEvents(new List<IEvent> { createdAlready });
            
            var created = new PathCreatedEvent {
                FeatureAdded = "bob",
                Path = "let/me/show",
            };

            var aggregate = await this
                .GivenAggregate(reader.Object, client.Object)
                .WithLoad();

            await aggregate
                .WhenPublishing(created)
                .ThenWePublish(client, created);
                
            aggregate.Paths.Select(_ => _.Name).Should()
                .BeEquivalentTo(new List<string> { created.Path, createdAlready.Path });
        }
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