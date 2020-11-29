using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using EventStore.Client;
using Feats.CQRS.Events;
using Feats.CQRS.Streams;
using Feats.Domain.Events;
using Feats.Domain.Strategies;
using Feats.EventStore;
using Feats.Management.Tests.EventStoreSetups.TestExtensions;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Feats.Management.Tests.Features
{
    public class StrategyAssignedEventTests : FeaturesAggregateTests
    {
        private readonly IStream _featureStream = new FeatureStream();

        [Test]
        public async Task GivenNoMatchingFeatures_WhenLoading_ThenWeDontUpdateTheCreatedEvent()
        {
            var created = new FeatureCreatedEvent {
                Name = "🦝",
                Path = "let/me/show/you",
            };

            var assigned = new StrategyAssignedEvent {
                Name = "bob",
                Path = "let/me/show/you",
                StrategyName = StrategyNames.IsOn,
                Settings = "settings",
            };

            var client = this.GivenIEventStoreClient()
                .WithAppendToStreamAsync(this._featureStream);

            var reader = this.GivenIReadStreamedEvents<FeatureStream>()
                .WithEvents(new List<IEvent> { created, assigned });

            var aggregate = await this
                .GivenAggregate(reader.Object, client.Object)
                .WithLoad();
                            
            var features = aggregate.Features.ToList();
            features.Should().Contain(_ => _.Name == created.Name);
            features.Should().NotContain(_ => _.Name == assigned.Name);
        }

        [Test]
        public async Task GivenAMatchingFeature_WhenLoadingStrategyAssigned_ThenWeupdateTheFeature()
        {
            var notMe = new FeatureCreatedEvent {
                Name = "🌲",
                Path = "let/me/show/you",
            };

            var created = new FeatureCreatedEvent {
                Name = "bob",
                Path = "let/me/show/you",
            };

            var assigned = new StrategyAssignedEvent {
                Name = "bob",
                Path = "let/me/show/you",
                StrategyName = StrategyNames.IsOn,
                Settings = "settings",
            };

            var reader = this.GivenIReadStreamedEvents<FeatureStream>()
                .WithEvents(new List<IEvent> { created, notMe, assigned });
            
            var client = this.GivenIEventStoreClient()
                .WithAppendToStreamAsync(this._featureStream);

            var aggregate = await this
                .GivenAggregate(reader.Object, client.Object)
                .WithLoad();

            var features = aggregate.Features.ToList();

            features.Select(_ => _.Name).Should()
                .BeEquivalentTo(new List<string> { created.Name, notMe.Name });

            features.Where(_ => _.Name == assigned.Name)
                .SelectMany(_ => _.Strategies.Keys)
                .Should()
                .BeEquivalentTo(new List<string> { StrategyNames.IsOn });
        }
        
        [Test]
        public async Task GivenNoFeatures_WhenPublishingStrategyAssignedEvent_ThenWePublishEventThoguhIDontExist()
        {
            var client = this.GivenIEventStoreClient()
                .WithAppendToStreamAsync(this._featureStream);

            var reader = this.GivenIReadStreamedEvents<FeatureStream>()
                .WithEvents(Enumerable.Empty<IEvent>());

            var assigned = new StrategyAssignedEvent {
                Name = "bob",
                Path = "let/me/show/you",
                StrategyName = StrategyNames.IsOn,
                Settings = "settings",
            };

            var aggregate = await this
                .GivenAggregate(reader.Object, client.Object)
                .WithLoad();

            await aggregate
                .WhenPublishing(assigned)
                .ThenWePublish(client, assigned);

            aggregate.Features.Should().BeEmpty();
        }

        [Test]
        public async Task GivenNoMatchingFeatures_WhenPublishingStrategyAssigned_ThenWePublishEventThoguhIDontExist()
        {
            var created = new FeatureCreatedEvent {
                Name = "🦝",
                Path = "let/me/show/you",
            };

            var client = this.GivenIEventStoreClient()
                .WithAppendToStreamAsync(this._featureStream);

            var reader = this.GivenIReadStreamedEvents<FeatureStream>()
                .WithEvents(new List<IEvent> { created });

            var assigned = new StrategyAssignedEvent {
                Name = "bob",
                Path = "let/me/show/you",
                StrategyName = StrategyNames.IsOn,
                Settings = "settings",
            };

            var aggregate = await this
                .GivenAggregate(reader.Object, client.Object)
                .WithLoad();

            await aggregate
                .WhenPublishing(assigned)
                .ThenWePublish(client, assigned);
                
            var features = aggregate.Features.ToList();
            features.Should().Contain(_ => _.Name == created.Name);
            features.Should().NotContain(_ => _.Name == assigned.Name);
        }

        [Test]
        public async Task GivenAMatchingFeature_WhenPublishingStrategyAssigned_ThenWePublishTheFeature()
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
            
            var assigned = new StrategyAssignedEvent {
                Name = "bob",
                Path = "let/me/show/you",
                StrategyName = StrategyNames.IsOn,
                Settings = "settings",
            };

            var aggregate = await this
                .GivenAggregate(reader.Object, client.Object)
                .WithLoad();

            await aggregate
                .WhenPublishing(assigned)
                .ThenWePublish(client, assigned);

            var features = aggregate.Features.ToList();

            features.Select(_ => _.Name).Should()
                .BeEquivalentTo(new List<string> { created.Name, notMe.Name });

            features.Where(_ => _.Name == assigned.Name)
                .SelectMany(_ => _.Strategies.Keys)
                .Should()
                .BeEquivalentTo(new List<string> { StrategyNames.IsOn });
        }
    }
        
    public static class StrategyAssignedEventTestsExtensions
    {
        public static async Task ThenWePublish(
            this Func<Task> funk,
            Mock<IEventStoreClient> mockedClient,
            StrategyAssignedEvent e)
        {
            await funk();
            
            mockedClient.Verify(
                _ => _.AppendToStreamAsync(
                    It.IsAny<FeatureStream>(),
                    It.IsAny<StreamState>(),
                    It.Is<IEnumerable<EventData>>(items => 
                        items.All(ed =>
                            ed.Type.Equals(EventTypes.StrategyAssigned) && 
                            JsonSerializer.Deserialize<StrategyAssignedEvent>(ed.Data.ToArray(), null).Name.Equals(e.Name, StringComparison.InvariantCultureIgnoreCase)
                        )),
                    It.IsAny<Action<EventStoreClientOperationOptions>?>(),
                    It.IsAny<UserCredentials?>(),
                    It.IsAny<CancellationToken>()),
                Times.Once());
        }
    }
}