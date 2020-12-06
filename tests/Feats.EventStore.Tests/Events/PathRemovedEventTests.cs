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
using Feats.EventStore;
using Feats.EventStore.Tests.TestExtensions;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Feats.Management.Tests.Paths
{
    public class PathRemovedEventTests : PathsAggregateTests
    {
        private readonly IStream _pathStream = new PathStream();

        [Test]
        public async Task GivenOtherPaths_WhenPublishingPathRemovedEvent_ThenWePublish()
        {
            var created = new PathRemovedEvent {
                FeatureRemoved = "derpy",
                Path = "derpy/wants/muffins",
            };
            
            var client = this.GivenIEventStoreClient()
                .WithAppendToStreamAsync(this._pathStream);

            var reader = this.GivenIReadStreamedEvents<PathStream>()
                .WithEvents(new List<IEvent> { created });

            var removing = new PathRemovedEvent {
                FeatureRemoved = "bob",
                Path = "let.me",
            };

            var aggregate = await this
                .GivenAggregate(reader.Object, client.Object)
                .WithLoad();

            await aggregate
                .WhenPublishing(removing)
                .ThenWePublish(client, removing);
        }

        [Test]
        public async Task GivenNoPreviousPaths_WhenPublishingPathRemovedEvent_ThenWePublish()
        {
            var client = this.GivenIEventStoreClient()
                .WithAppendToStreamAsync(this._pathStream);

            var reader = this.GivenIReadStreamedEvents<PathStream>()
                .WithEvents(Enumerable.Empty<IEvent>());

            var removed = new PathRemovedEvent {
                FeatureRemoved = "bob",
                Path = "let/me/show/you",
            };

            var aggregate = await this
                .GivenAggregate(reader.Object, client.Object)
                .WithLoad();

            await aggregate
                .WhenPublishing(removed)
                .ThenWePublish(client, removed);

            aggregate.Paths.Select(_ => _.Name)
                .Should()
                .BeEquivalentTo(Enumerable.Empty<string>());
        }
        
        [Test]
        public async Task GivenPreExistingPaths_WhenRemovingThePath_ThenThePathsAreUpdated()
        {
            var created = new PathCreatedEvent {
                FeatureAdded = "bob",
                Path = "let/me/show/you",
            };
            
            var createdSecond = new PathCreatedEvent {
                FeatureAdded = "bob",
                Path = "let/me/show",
            };
            
            var createdThird = new PathCreatedEvent {
                FeatureAdded = "bob",
                Path = "let/me",
            };

            var client = this.GivenIEventStoreClient()
                .WithAppendToStreamAsync(this._pathStream);

            var reader = this.GivenIReadStreamedEvents<PathStream>()
                .WithEvents(new List<IEvent> { created, createdSecond, createdThird });
            
            var removed = new PathRemovedEvent {
                FeatureRemoved = "bob",
                Path = "let/me/show/you",
            };
            
            var aggregate = await this
                .GivenAggregate(reader.Object, client.Object)
                .WithLoad();

            await aggregate
                .WhenPublishing(removed)
                .ThenWePublish(client, removed);
                
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
                        TotalFeatures = 1,
                    },
                });
        }
        
        [Test]
        public async Task GivenOnePreExistingPath_WhenRemovingThePath_ThenThePathsAreEmptied()
        {
            var created = new PathCreatedEvent {
                FeatureAdded = "bob",
                Path = "let/me/show/you",
            };

            var removed = new PathRemovedEvent {
                FeatureRemoved = "bob",
                Path = "let/me/show/you",
            };

            var client = this.GivenIEventStoreClient()
                .WithAppendToStreamAsync(this._pathStream);

            var reader = this.GivenIReadStreamedEvents<PathStream>()
                .WithEvents(new List<IEvent> { created, removed });
            
            var aggregate = await this
                .GivenAggregate(reader.Object, client.Object)
                .WithLoad();

            aggregate.Paths.Should().BeEmpty();
        }
                
        [Test]
        public async Task GivenOnePreExistingPath_WhenRemovingAnUnknownPath_ThenThePathIsUntouched()
        {
            var created = new PathCreatedEvent {
                FeatureAdded = "bob",
                Path = "let/me/show/you",
            };

            var removed = new PathRemovedEvent {
                FeatureRemoved = "bob",
                Path = "let/me/show/arrrrgh",
            };

            var client = this.GivenIEventStoreClient()
                .WithAppendToStreamAsync(this._pathStream);

            var reader = this.GivenIReadStreamedEvents<PathStream>()
                .WithEvents(new List<IEvent> { created, removed });
            
            var aggregate = await this
                .GivenAggregate(reader.Object, client.Object)
                .WithLoad();
            
            aggregate.Paths.Should()
                .BeEquivalentTo(new List<Path> 
                {
                    new Path 
                    {
                        Name = "let",
                        TotalFeatures = 1,
                    },
                    new Path 
                    {
                        Name = "let/me",
                        TotalFeatures = 1,
                    },
                    new Path 
                    {
                        Name = "let/me/show",
                        TotalFeatures = 1,
                    },
                    new Path 
                    {
                        Name = "let/me/show/you",
                        TotalFeatures = 1,
                    },
                });
        }
    }

    public static class PathRemovedEventTestExtensions
    {
        public static async Task ThenWePublish(
            this Func<Task> funk,
            Mock<IEventStoreClient> mockedClient,
            PathRemovedEvent e)
        {
            await funk();

            mockedClient.Verify(
                _ => _.AppendToStreamAsync(
                    It.IsAny<PathStream>(),
                    It.IsAny<StreamState>(),
                    It.Is<IEnumerable<EventData>>(items => 
                        items.All(ed =>
                            ed.Type.Equals(EventTypes.PathRemoved) && 
                            JsonSerializer.Deserialize<PathRemovedEvent>(ed.Data.ToArray(), null).Path.Equals(e.Path, StringComparison.InvariantCultureIgnoreCase)
                        )),
                    It.IsAny<Action<EventStoreClientOperationOptions>?>(),
                    It.IsAny<UserCredentials?>(),
                    It.IsAny<CancellationToken>()),
                Times.Once());
        }
    }
}