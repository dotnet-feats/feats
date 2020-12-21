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
using Feats.Domain.Aggregates;
using Feats.Domain.Events;
using Feats.EventStore.Exceptions;
using Feats.EventStore.Tests.Aggregates;
using Feats.EventStore.Tests.TestExtensions;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Feats.EventStore.Tests.Events
{
    public class FeatureUpdatedEventTests : FeaturesAggregateTests
    {
        private readonly IStream _featureStream = new FeatureStream();
        
        [Test]
        public async Task GivenNoFeatures_WhenPublishingFeatureUpdatedEvent_ThenWeThrowFeatureNotFoundExcepion()
        {
            var client = this.GivenIEventStoreClient()
                .WithAppendToStreamAsync(this._featureStream);

            var reader = this.GivenIReadStreamedEvents<FeatureStream>()
                .WithEvents(Enumerable.Empty<IEvent>());

            var updated = new FeatureUpdatedEvent {
                Name = "bob",
                Path = "let/me/show/you",
            };

            var aggregate = await this
                .GivenAggregate(reader.Object, client.Object)
                .WithLoad()();

            await aggregate
                .WhenPublishing(updated)
                .ThenExceptionIsThrown<FeatureNotFoundException>();
        }
        
        [Test]
        public async Task GivenDraftFeature_WhenPublishingFeatureUpdatedEvent_ThenWePublishTheEvent()
        {
            var createdAlready = new FeatureCreatedEvent {
                Name = "bob",
                Path = "let/me/show/you",
            };
            
            var client = this.GivenIEventStoreClient()
                .WithAppendToStreamAsync(this._featureStream);

            var reader = this.GivenIReadStreamedEvents<FeatureStream>()
                .WithEvents(new List<IEvent> { createdAlready });
            
            var updated = new FeatureUpdatedEvent {
                Name = "bob",
                Path = "let/me/show/you",
                NewName = "bob2",
                NewPath = "let",
            };

            var aggregate = await this
                .GivenAggregate(reader.Object, client.Object)
                .WithLoad()();

            await aggregate
                .WhenPublishing(updated)
                .ThenWePublish(client, updated)
                .ThenFeatureIsUpdated(aggregate, updated);
        }

    
        [Test]
        public async Task GivenFeaturePublishedBefore_WhenPublishingFeatureUpdatedEvent_ThenWeThrowFeatureWasPublishedBefoeException()
        {
            var createdAlready = new FeatureCreatedEvent {
                Name = "bob",
                Path = "let/me/show/you",
                
            };
            var publishedAlready = new FeaturePublishedEvent {
                Name = createdAlready.Name,
                Path = createdAlready.Path,
                PublishedBy = "moua",
            };
            var client = this.GivenIEventStoreClient()
                .WithAppendToStreamAsync(this._featureStream);

            var reader = this.GivenIReadStreamedEvents<FeatureStream>()
                .WithEvents(new List<IEvent> { createdAlready, publishedAlready });
            
            var updated = new FeatureUpdatedEvent {
                Name = createdAlready.Name,
                Path = createdAlready.Path,
                NewName = "bob2",
                NewPath = "let",
            };

            var aggregate = await this
                .GivenAggregate(reader.Object, client.Object)
                .WithLoad()();

            await aggregate
                .WhenPublishing(updated)
                .ThenExceptionIsThrown<FeatureIsNotInDraftException>();
        }
    }

    public static class FeatureUpdatedEventTestsExtensions
    {
        public static async Task ThenWePublish(
            this Func<Task> funk,
            Mock<IEventStoreClient> mockedClient,
            FeatureUpdatedEvent e)
        {
            await funk();
            
            mockedClient.Verify(
                _ => _.AppendToStreamAsync(
                    It.IsAny<FeatureStream>(),
                    It.IsAny<StreamState>(),
                    It.Is<IEnumerable<EventData>>(items => 
                        items.All(ed =>
                            ed.Type.Equals(EventTypes.FeatureUpdated) && 
                            JsonSerializer.Deserialize<FeatureUpdatedEvent>(ed.Data.ToArray(), null)!.Name.Equals(e.Name, StringComparison.InvariantCultureIgnoreCase)
                        )),
                    It.IsAny<Action<EventStoreClientOperationOptions>?>(),
                    It.IsAny<UserCredentials?>(),
                    It.IsAny<CancellationToken>()),
                Times.Once());
        }

        public static async Task ThenFeatureIsUpdated(
            this Task task, 
            IFeaturesAggregate aggregate, 
            FeatureUpdatedEvent updatedEvent)
        {
            await task;
            
            aggregate.Features.Select(_ => _.Name).Should().BeEquivalentTo(new List<string> 
            {
                updatedEvent.NewName
            });

            aggregate.Features.Select(_ => _.Path).Should().BeEquivalentTo(new List<string> 
            {
                updatedEvent.NewPath
            });
        }
    }
}