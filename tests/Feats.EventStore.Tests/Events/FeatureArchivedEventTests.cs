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
using Feats.Domain;
using Feats.Domain.Events;
using Feats.EventStore.Exceptions;
using Feats.EventStore.Tests.Aggregates;
using Feats.EventStore.Tests.TestExtensions;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Feats.EventStore.Tests.Events
{
    public class FeatureArchivedEventTests : FeaturesAggregateTests
    {
        private readonly IStream _featureStream = new FeatureStream();


        [Test]
        public async Task GivenOnlyArchivedEvent_WhenLoading_ThenWeGetEmptyFeaturesList()
        {
            var archived = new FeatureArchivedEvent {
                Name = "bob",
                Path = "let/me/show/you",
            };

            var reader = this.GivenIReadStreamedEvents<FeatureStream>()
                .WithEvents(new List<IEvent> { archived });

            var client = this.GivenIEventStoreClient()
                .WithAppendToStreamAsync(this._featureStream);

            await this
                .GivenAggregate(reader.Object, client.Object)
                .WithLoad()
                .ThenExceptionIsThrown<FeatureNotFoundException>();
        }

        [Test]
        public async Task GivenNoMatchingArchivedFeatures_WhenLoading_ThenWeThrowFeatureNotFoundException()
        {
            var created = new FeatureCreatedEvent {
                Name = "🦝",
                Path = "let/me/show/you",
            };
            
            var archived = new FeatureArchivedEvent {
                Name = "bob",
                Path = "let/me/show/you",
            };

            var reader = this.GivenIReadStreamedEvents<FeatureStream>()
                .WithEvents(new List<IEvent> { created, archived });

            var client = this.GivenIEventStoreClient()
                .WithAppendToStreamAsync(this._featureStream);

            await this
                .GivenAggregate(reader.Object, client.Object)
                .WithLoad()
                .ThenExceptionIsThrown<FeatureNotFoundException>();
        }

        [Test]
        public async Task GivenAMatchingFeature_WhenLoading_ThenWeGetAArchivedFeature()
        {
            var createdNotMatching = new FeatureCreatedEvent {
                Name = "🤚",
                Path = "🌲/",
            };
            
            var created = new FeatureCreatedEvent {
                Name = "🦝",
                Path = "🌲/",
            };
            
            var archived = new FeatureArchivedEvent {
                Name = "🦝",
                Path = "🌲/",
            };

            var reader = this.GivenIReadStreamedEvents<FeatureStream>()
                .WithEvents(new List<IEvent> { created, createdNotMatching, archived });

            var client = this.GivenIEventStoreClient()
                .WithAppendToStreamAsync(this._featureStream);

            var aggregate = await this
                .GivenAggregate(reader.Object, client.Object)
                .WithLoad()();

            var features = aggregate.Features.ToList();

            features.Select(_ => _.Name).Should()
                .BeEquivalentTo(new List<string> { created.Name, createdNotMatching.Name });
            features.Where(_ => _.Name == archived.Name).Select(_ => _.State)
                .Should()
                .BeEquivalentTo(new List<FeatureState> { FeatureState.Archived });
            features.Where(_ => _.Name != archived.Name).Select(_ => _.State)
                .Should()
                .BeEquivalentTo(new List<FeatureState> { FeatureState.Draft });
        }

        [Test]
        public async Task GivenAMatchingFeature_WhenPublishingFeatureArchivedEvent_ThenWePublishTheFeature()
        {
            var notMe = new FeatureCreatedEvent {
                Name = "🌲",
                Path = "let/me/show/you",
            };

            var created = new FeatureCreatedEvent {
                Name = "bob",
                Path = "let/me/show/you",
            };
            
            var client = this.GivenIEventStoreClient()
                .WithAppendToStreamAsync(this._featureStream);

            var reader = this.GivenIReadStreamedEvents<FeatureStream>()
                .WithEvents(new List<IEvent> { created, notMe });
            
            var aggregate = await this
                .GivenAggregate(reader.Object, client.Object)
                .WithLoad()();

            var archived = new FeatureArchivedEvent {
                Name = "bob",
                Path = "let/me/show/you",
            };

            await aggregate
                .WhenPublishing(archived)
                .ThenWePublish(client, archived);
                
            var features = aggregate.Features.ToList();
            features.Select(_ => _.Name).Should()
                .BeEquivalentTo(new List<string> { created.Name, notMe.Name });
            features.Where(_ => _.Name == archived.Name).Select(_ => _.State)
                .Should()
                .BeEquivalentTo(new List<FeatureState> { FeatureState.Archived });
            features.Where(_ => _.Name != archived.Name).Select(_ => _.State)
                .Should()
                .BeEquivalentTo(new List<FeatureState> { FeatureState.Draft });
        }
        

        [Test]
        public async Task GivenNoMatchingArchivedFeatures_WhenPublishing_ThenWeTHrowFeatureNotFoundException()
        {
            var created = new FeatureCreatedEvent {
                Name = "🦝",
                Path = "let/me/show/you",
            };
            
            var reader = this.GivenIReadStreamedEvents<FeatureStream>()
                .WithEvents(new List<IEvent> { created });

            var client = this.GivenIEventStoreClient()
                .WithAppendToStreamAsync(this._featureStream);
            
            var aggregate = await this
                .GivenAggregate(reader.Object, client.Object)
                .WithLoad()();

            var archived = new FeatureArchivedEvent {
                Name = "bob",
                Path = "let/me/show/you",
            };

            await aggregate
                .WhenPublishing(archived)
                .ThenExceptionIsThrown<FeatureNotFoundException>();
        }
    }
        
    public static class FeatureArchivedEventTestsExtensions
    {
        public static async Task ThenWePublish(
            this Func<Task> funk,
            Mock<IEventStoreClient> mockedClient,
            FeatureArchivedEvent e)
        {
            await funk();
            
            mockedClient.Verify(
                _ => _.AppendToStreamAsync(
                    It.IsAny<FeatureStream>(),
                    It.IsAny<StreamState>(),
                    It.Is<IEnumerable<EventData>>(items => 
                        items.All(ed =>
                            ed.Type.Equals(EventTypes.FeatureArchived) && 
                            JsonSerializer.Deserialize<FeatureArchivedEvent>(ed.Data.ToArray(), null)!.Name.Equals(e.Name, StringComparison.InvariantCultureIgnoreCase)
                        )),
                    It.IsAny<Action<EventStoreClientOperationOptions>?>(),
                    It.IsAny<UserCredentials?>(),
                    It.IsAny<CancellationToken>()),
                Times.Once());
        }
    }
}