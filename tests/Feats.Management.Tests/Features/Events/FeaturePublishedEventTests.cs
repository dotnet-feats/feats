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
using Feats.Management.Tests.EventStoreSetups.TestExtensions;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Feats.Management.Tests.Features
{
    public class FeaturePublishedEventTests : FeaturesAggregateTests
    {
        private readonly IStream _featureStream = new FeatureStream();


        [Test]
        public async Task GivenOnlyPublishedEvent_WhenLoading_ThenWeGetEmptyFeaturesList()
        {
            var published = new FeaturePublishedEvent {
                Name = "bob",
                Path = "let/me/show/you",
            };

            var reader = this.GivenIReadStreamedEvents<FeatureStream>()
                .WithEvents(new List<IEvent> { published });

            var client = this.GivenIEventStoreClient()
                .WithAppendToStreamAsync(this._featureStream);

            var aggregate = await this
                .GivenAggregate(reader.Object, client.Object)
                .WithLoad();

            aggregate.Features.Should().BeEmpty();
        }

        [Test]
        public async Task GivenNoMatchingPublishedFeatures_WhenLoading_ThenWeGetONlyCreatedFeatures()
        {
            var created = new FeatureCreatedEvent {
                Name = "🦝",
                Path = "let/me/show/you",
            };
            
            var published = new FeaturePublishedEvent {
                Name = "bob",
                Path = "let/me/show/you",
            };

            var reader = this.GivenIReadStreamedEvents<FeatureStream>()
                .WithEvents(new List<IEvent> { created, published });

            var client = this.GivenIEventStoreClient()
                .WithAppendToStreamAsync(this._featureStream);

            var aggregate = await this
                .GivenAggregate(reader.Object, client.Object)
                .WithLoad();

            var features = aggregate.Features.ToList();

            features.Should().Contain(_ => _.Name == created.Name);
            features.Should().NotContain(_ => _.Name == published.Name);
        }

        [Test]
        public async Task GivenAMatchingFeature_WHenLoading_ThenWeGetAPublishedFeature()
        {
            var createdNotMatching = new FeatureCreatedEvent {
                Name = "🤚",
                Path = "🌲/",
            };
            
            var created = new FeatureCreatedEvent {
                Name = "🦝",
                Path = "🌲/",
            };
            
            var published = new FeaturePublishedEvent {
                Name = "🦝",
                Path = "🌲/",
            };

            var reader = this.GivenIReadStreamedEvents<FeatureStream>()
                .WithEvents(new List<IEvent> { created, createdNotMatching, published });

            var client = this.GivenIEventStoreClient()
                .WithAppendToStreamAsync(this._featureStream);

            var aggregate = await this
                .GivenAggregate(reader.Object, client.Object)
                .WithLoad();

            var features = aggregate.Features.ToList();

            features.Select(_ => _.Name).Should()
                .BeEquivalentTo(new List<string> { created.Name, createdNotMatching.Name });
            features.Where(_ => _.Name == published.Name).Select(_ => _.State)
                .Should()
                .BeEquivalentTo(new List<FeatureState> { FeatureState.Published });
            features.Where(_ => _.Name != published.Name).Select(_ => _.State)
                .Should()
                .BeEquivalentTo(new List<FeatureState> { FeatureState.Draft });
        }

        [Test]
        public async Task GivenNoFeatures_WhenPublishingFeaturePublijshedEvent_ThenWePublishEventThoguhIDontExist()
        {
            var client = this.GivenIEventStoreClient()
                .WithAppendToStreamAsync(this._featureStream);

            var reader = this.GivenIReadStreamedEvents<FeatureStream>()
                .WithEvents(Enumerable.Empty<IEvent>());

            var published = new FeaturePublishedEvent {
                Name = "bob",
                Path = "let/me/show/you",
            };

            var aggregate = await this
                .GivenAggregate(reader.Object, client.Object)
                .WithLoad();

            await aggregate
                .WhenPublishing(published)
                .ThenWePublish(client, published);

            aggregate.Features.Should().BeEmpty();
        }

        [Test]
        public async Task GivenNoMatchingFeatures_WhenPublishingFeaturePublijshedEvent_ThenWePublishEventThoguhIDontExist()
        {
            var created = new FeatureCreatedEvent {
                Name = "🦝",
                Path = "let/me/show/you",
            };

            var client = this.GivenIEventStoreClient()
                .WithAppendToStreamAsync(this._featureStream);

            var reader = this.GivenIReadStreamedEvents<FeatureStream>()
                .WithEvents(new List<IEvent> { created });

            var published = new FeaturePublishedEvent {
                Name = "bob",
                Path = "let/me/show/you",
            };

            var aggregate = await this
                .GivenAggregate(reader.Object, client.Object)
                .WithLoad();

            await aggregate
                .WhenPublishing(published)
                .ThenWePublish(client, published);
                
            var features = aggregate.Features.ToList();
            features.Should().Contain(_ => _.Name == created.Name);
            features.Should().NotContain(_ => _.Name == published.Name);
        }

        [Test]
        public async Task GivenAMatchingFeature_WhenPublishingFeaturePublishedEvent_ThenWePublishTheFeature()
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
            
            var published = new FeaturePublishedEvent {
                Name = "bob",
                Path = "let/me/show/you",
            };

            var aggregate = await this
                .GivenAggregate(reader.Object, client.Object)
                .WithLoad();

            await aggregate
                .WhenPublishing(published)
                .ThenWePublish(client, published);
                
            var features = aggregate.Features.ToList();
            features.Select(_ => _.Name).Should()
                .BeEquivalentTo(new List<string> { created.Name, notMe.Name });
            features.Where(_ => _.Name == published.Name).Select(_ => _.State)
                .Should()
                .BeEquivalentTo(new List<FeatureState> { FeatureState.Published });
            features.Where(_ => _.Name != published.Name).Select(_ => _.State)
                .Should()
                .BeEquivalentTo(new List<FeatureState> { FeatureState.Draft });
        }
    }
        
    public static class FeaturePublishedEventTestsExtensions
    {
        public static async Task ThenWePublish(
            this Func<Task> funk,
            Mock<IEventStoreClient> mockedClient,
            FeaturePublishedEvent e)
        {
            await funk();
            
            mockedClient.Verify(
                _ => _.AppendToStreamAsync(
                    It.IsAny<FeatureStream>(),
                    It.IsAny<StreamState>(),
                    It.Is<IEnumerable<EventData>>(items => 
                        items.All(ed =>
                            ed.Type.Equals(EventTypes.FeaturePublished) && 
                            JsonSerializer.Deserialize<FeaturePublishedEvent>(ed.Data.ToArray(), null).Name.Equals(e.Name, StringComparison.InvariantCultureIgnoreCase)
                        )),
                    It.IsAny<Action<EventStoreClientOperationOptions>?>(),
                    It.IsAny<UserCredentials?>(),
                    It.IsAny<CancellationToken>()),
                Times.Once());
        }
    }
}