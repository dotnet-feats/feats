using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using EventStore.Client;
using Feats.CQRS.Events;
using Feats.CQRS.Streams;
using Feats.Domain;
using Feats.Domain.Events;
using Feats.EventStore.Tests.Aggregates;
using Feats.EventStore.Tests.TestExtensions;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Feats.EventStore.Tests.Events
{
    public class PathCreatedEventTests : PathsAggregateTests
    {
        private readonly IStream _pathStream = new PathStream();

        [Test]
        public async Task GivenOtherPaths_WhenPublishingPathCreatedEvent_ThenWePublish()
        {
            var createdAlready = new PathCreatedEvent {
                FeatureAdded = "derpy",
                Path = "derpy/wants/muffins",
            };
            
            var client = this.GivenIEventStoreClient()
                .WithAppendToStreamAsync(this._pathStream);

            var reader = this.GivenIReadStreamedEvents<PathStream>()
                .WithEvents(new List<IEvent> { createdAlready });

            var created = new PathCreatedEvent {
                FeatureAdded = "bob",
                Path = "let.me",
            };

            var aggregate = await this
                .GivenAggregate(reader.Object, client.Object)
                .WithLoad();

            await aggregate
                .WhenPublishing(created)
                .ThenWePublish(client, created);

            aggregate.Paths.Select(_ => _.Name)
                .Should()
                .BeEquivalentTo(new List<string> { 
                    "derpy",
                    "derpy/wants", 
                    "derpy/wants/muffins",
                    "let", 
                    "let.me"
                });
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

            aggregate.Paths.Select(_ => _.Name)
                .Should()
                .BeEquivalentTo(new List<string> { 
                    "let",
                    "let/me", 
                    "let/me/show", 
                    "let/me/show/you"
                });
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
                
            aggregate.Paths.Should()
                .BeEquivalentTo(new List<Path> 
                {
                    new Path 
                    {
                        Name = "let",
                        TotalFeatures = 2,
                    },
                    new Path 
                    {
                        Name = "let/me",
                        TotalFeatures = 2,
                    },
                    new Path 
                    {
                        Name = "let/me/show",
                        TotalFeatures = 2,
                    },
                    new Path 
                    {
                        Name = "let/me/show/you",
                        TotalFeatures = 1,
                    },
                });
        }
        
        
        [Test]
        public async Task GivenAdditionalPath_WhenPublishingPathCreatedEvent_ThenWePublishTheFeature()
        {
            var createdAlready = new PathCreatedEvent {
                FeatureAdded = "bob",
                Path = "let/me/show",
            };
            
            var client = this.GivenIEventStoreClient()
                .WithAppendToStreamAsync(this._pathStream);

            var reader = this.GivenIReadStreamedEvents<PathStream>()
                .WithEvents(new List<IEvent> { createdAlready });
            
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
                
            aggregate.Paths.Should()
                .BeEquivalentTo(new List<Path> 
                {
                    new Path 
                    {
                        Name = "let",
                        TotalFeatures = 2,
                    },
                    new Path 
                    {
                        Name = "let/me",
                        TotalFeatures = 2,
                    },
                    new Path 
                    {
                        Name = "let/me/show",
                        TotalFeatures = 2,
                    },
                    new Path 
                    {
                        Name = "let/me/show/you",
                        TotalFeatures = 1,
                    },
                });
        }
    }

    public static class PathCreatdEventTestExtensions
    {
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
                            JsonSerializer.Deserialize<PathCreatedEvent>(ed.Data.ToArray(), null)!.Path.Equals(e.Path, StringComparison.InvariantCultureIgnoreCase)
                        )),
                    It.IsAny<Action<EventStoreClientOperationOptions>?>(),
                    It.IsAny<UserCredentials?>(),
                    It.IsAny<CancellationToken>()),
                Times.Once());
        }
    }
}