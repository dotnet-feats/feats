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
using Feats.Domain.Events;
using Feats.Domain.Strategies;
using Feats.EventStore.Exceptions;
using Feats.EventStore.Tests.Aggregates;
using Feats.EventStore.Tests.TestExtensions;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Feats.EventStore.Tests.Events
{
    public class StrategyUnassignedEventTests : FeaturesAggregateTests
    {
        private readonly IStream _featureStream = new FeatureStream();

        [Test]
        public async Task GivenAMatchingFeature_WhenLoadingStrategyUnassigned_ThenWeupdateTheFeature()
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
            
            var unassigned = new StrategyUnAssignedEvent {
                Name = "bob",
                Path = "let/me/show/you",
                StrategyName = assigned.StrategyName,
            };

            var reader = this.GivenIReadStreamedEvents<FeatureStream>()
                .WithEvents(new List<IEvent> { created, notMe, assigned, unassigned });
            
            var client = this.GivenIEventStoreClient()
                .WithAppendToStreamAsync(this._featureStream);

            var aggregate = await this
                .GivenAggregate(reader.Object, client.Object)
                .WithLoad()();

            var features = aggregate.Features.ToList();

            features.Select(_ => _.Name).Should()
                .BeEquivalentTo(new List<string> { created.Name, notMe.Name });

            features.Where(_ => _.Name == unassigned.Name)
                .SelectMany(_ => _.Strategies)
                .Should()
                .BeEmpty();
        }
        
        [Test]
        public async Task GivenAMatchingFeatureButNOMatchingStrategy_WhenLoadingStrategyUnassigned_ThenWeChangeNothing()
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
                Name = created.Name,
                Path = created.Path,
                StrategyName = "yolo",
                Settings = "settings",
            };
            
            var unassigned = new StrategyUnAssignedEvent {
                Name = created.Name,
                Path = created.Path,
                StrategyName = StrategyNames.IsOn,
            };

            var reader = this.GivenIReadStreamedEvents<FeatureStream>()
                .WithEvents(new List<IEvent> { created, notMe, assigned, unassigned });
            
            var client = this.GivenIEventStoreClient()
                .WithAppendToStreamAsync(this._featureStream);

            var aggregate = await this
                .GivenAggregate(reader.Object, client.Object)
                .WithLoad()();

            var features = aggregate.Features.ToList();

            features.Select(_ => _.Name).Should()
                .BeEquivalentTo(new List<string> { created.Name, notMe.Name });

            features.Where(_ => _.Name == unassigned.Name)
                .SelectMany(_ => _.Strategies.Select(s => s.Key))
                .Should()
                .BeEquivalentTo(new List<string> 
                {
                    "yolo",
                });
        }
        
        [Test]
        public async Task GivenAMatchingFeatureWithNoStrategies_WhenLoadingStrategyUnassigned_ThenWeupdateTheFeature()
        {
            var notMe = new FeatureCreatedEvent {
                Name = "🌲",
                Path = "let/me/show/you",
            };

            var created = new FeatureCreatedEvent {
                Name = "bob",
                Path = "let/me/show/you",
            };
            
            var unassigned = new StrategyUnAssignedEvent {
                Name = "bob",
                Path = "let/me/show/you",
                StrategyName = StrategyNames.IsOn,
            };

            var reader = this.GivenIReadStreamedEvents<FeatureStream>()
                .WithEvents(new List<IEvent> { created, notMe, unassigned });
            
            var client = this.GivenIEventStoreClient()
                .WithAppendToStreamAsync(this._featureStream);

            var aggregate = await this
                .GivenAggregate(reader.Object, client.Object)
                .WithLoad()();

            var features = aggregate.Features.ToList();

            features.Select(_ => _.Name).Should()
                .BeEquivalentTo(new List<string> { created.Name, notMe.Name });

            features.Where(_ => _.Name == unassigned.Name)
                .SelectMany(_ => _.Strategies)
                .Should()
                .BeEmpty();
        }

        [Test]
        public async Task GivenNoFeatures_WhenPublishingStrategyAssignedEvent_ThenWePublishEventThoguhIDontExist()
        {
            var client = this.GivenIEventStoreClient()
                .WithAppendToStreamAsync(this._featureStream);

            var reader = this.GivenIReadStreamedEvents<FeatureStream>()
                .WithEvents(Enumerable.Empty<IEvent>());

            var unassigned = new StrategyUnAssignedEvent {
                Name = "bob",
                Path = "let/me/show/you",
                StrategyName = StrategyNames.IsOn,
            };

            var aggregate = await this
                .GivenAggregate(reader.Object, client.Object)
                .WithLoad()();

            await aggregate
                .WhenPublishing(unassigned)
                .ThenExceptionIsThrown<FeatureNotFoundException>();
        }
        
        [Test]
        public async Task GivenAPublishedFeature_WhenUnassigningAStrategy_ThenWeThrow()
        {
            var created = new FeatureCreatedEvent {
                Name = "bob",
                Path = "let/me/show/you",
            };

            var assigned = new StrategyAssignedEvent {
                Name = created.Name,
                Path = created.Path,
                StrategyName = "yolo",
                Settings = "settings",
            };

            var published = new FeaturePublishedEvent {
                Name = created.Name,
                Path = created.Path,
            };

            var unassigned = new StrategyUnAssignedEvent {
                Name = created.Name,
                Path = created.Path,
                StrategyName = StrategyNames.IsOn,
            };

            var client = this.GivenIEventStoreClient()
                .WithAppendToStreamAsync(this._featureStream);

            var reader = this.GivenIReadStreamedEvents<FeatureStream>()
                .WithEvents(new List<IEvent> {
                    created,
                    assigned,
                    published,
                    unassigned,
                });

            await this
                .GivenAggregate(reader.Object, client.Object)
                .WithLoad()
                .ThenExceptionIsThrown<FeatureIsNotInDraftException>();
        }
    }
        
    public static class StrategyUnassignedEventTestsExtensions
    {
        public static async Task ThenWePublish(
            this Func<Task> funk,
            Mock<IEventStoreClient> mockedClient,
            StrategyUnAssignedEvent e)
        {
            await funk();
            
            mockedClient.Verify(
                _ => _.AppendToStreamAsync(
                    It.IsAny<FeatureStream>(),
                    It.IsAny<StreamState>(),
                    It.Is<IEnumerable<EventData>>(items => 
                        items.All(ed =>
                            ed.Type.Equals(EventTypes.StrategyAssigned) && 
                            JsonSerializer.Deserialize<StrategyUnAssignedEvent>(ed.Data.ToArray(), null)!.Name.Equals(e.Name, StringComparison.InvariantCultureIgnoreCase)
                        )),
                    It.IsAny<Action<EventStoreClientOperationOptions>?>(),
                    It.IsAny<UserCredentials?>(),
                    It.IsAny<CancellationToken>()),
                Times.Once());
        }
    }
}