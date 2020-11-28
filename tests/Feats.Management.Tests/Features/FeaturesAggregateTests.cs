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
using Feats.Management.EventStoreSetups;
using Feats.Management.Features;
using Feats.Management.Features.Events;
using Feats.Management.Features.Exceptions;
using Feats.Management.Tests.EventStoreSetups.TestExtensions;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Feats.Management.Tests.Features
{
    public class FeaturesAggregateTests : TestBase
    {
        private readonly IStream _featureStream = new FeatureStream();

        [Test]
        public async Task GivenAConflictingFeature_WhenPublishingFeatureCreatedEvent_ThenWeThrowAConflictException()
        {
            var createdAlready = new FeatureCreatedEvent {
                Name = "bob",
                Path = "let/me/show/you",
            };

            var client = this.GivenIEventStoreClient()
                .WithAppendToStreamAsync(this._featureStream);

            var reader = this.GivenIReadStreamedEvents<FeatureStream>()
                .WithEvents(new List<IEvent> { createdAlready });

            var aggregate = await this
                .GivenAggregate(reader.Object, client.Object)
                .WithLoad();

            await aggregate
                .WhenPublishing(createdAlready)
                .ThenExceptionIsThrown<FeatureAlreadyExistsException>();
        }

        [Test]
        public async Task GivenNoFeatures_WhenPublishingFeatureCreatedEvent_ThenWePublishTheFeature()
        {
            var client = this.GivenIEventStoreClient()
                .WithAppendToStreamAsync(this._featureStream);

            var reader = this.GivenIReadStreamedEvents<FeatureStream>()
                .WithEvents(Enumerable.Empty<IEvent>());

            var created = new FeatureCreatedEvent {
                Name = "bob",
                Path = "let/me/show/you",
            };

            var aggregate = await this
                .GivenAggregate(reader.Object, client.Object)
                .WithLoad();

            await aggregate
                .WhenPublishing(created)
                .ThenWePublish(client, created);
            aggregate.Features.Select(_ => _.Name).Should()
                .BeEquivalentTo(new List<string> { created.Name });
        }
        
        [Test]
        public async Task GivenNotConflictingFeatures_WhenPublishingFeatureCreatedEvent_ThenWePublishTheFeature()
        {
            var createdAlready = new FeatureCreatedEvent {
                Name = "bob",
                Path = "let/me/show/you",
            };
            
            var client = this.GivenIEventStoreClient()
                .WithAppendToStreamAsync(this._featureStream);

            var reader = this.GivenIReadStreamedEvents<FeatureStream>()
                .WithEvents(new List<IEvent> { createdAlready });
            
            var created = new FeatureCreatedEvent {
                Name = "bob2",
                Path = "let/me/show/you",
            };

            var aggregate = await this
                .GivenAggregate(reader.Object, client.Object)
                .WithLoad();

            await aggregate
                .WhenPublishing(created)
                .ThenWePublish(client, created);
                
            aggregate.Features.Select(_ => _.Name).Should()
                .BeEquivalentTo(new List<string> { created.Name, createdAlready.Name });
        }
    }

    public static class FeaturesAggregateTestsExtensions
    {
        public static IFeaturesAggregate GivenAggregate(
            this FeaturesAggregateTests tests, 
            IReadStreamedEvents<FeatureStream> reader,
            IEventStoreClient client)
        {
            return new FeaturesAggregate(
                tests.GivenLogger<FeaturesAggregate>(), 
                reader, 
                client);
        }

        public static async Task<IFeaturesAggregate> WithLoad(this IFeaturesAggregate aggregate)
        {
            await aggregate.Load();

            return aggregate;
        }

        public static Func<Task> WhenPublishing(this IFeaturesAggregate aggregate, IEvent e)
        {
            return () => aggregate.Publish(e);
        }
        
        public static async Task ThenWePublish(
            this Func<Task> funk,
            Mock<IEventStoreClient> mockedClient,
            FeatureCreatedEvent e)
        {
            await funk();
/*
            mockedClient.Verify(
                _ => _.AppendToStreamAsync(
                    It.IsAny<PathStream>(),
                    It.IsAny<StreamState>(),
                    It.Is<IEnumerable<EventData>>(items => 
                        items.All(ed =>
                            ed.Type.Equals(EventTypes.PathCreated) && 
                            JsonSerializer.Deserialize<PathCreatedEvent>(ed.Data.ToArray(), null) != null
                        )),
                    It.IsAny<Action<EventStoreClientOperationOptions>?>(),
                    It.IsAny<UserCredentials?>(),
                    It.IsAny<CancellationToken>()),
                Times.Once());
                */
            mockedClient.Verify(
                _ => _.AppendToStreamAsync(
                    It.IsAny<FeatureStream>(),
                    It.IsAny<StreamState>(),
                    It.Is<IEnumerable<EventData>>(items => 
                        items.All(ed =>
                            ed.Type.Equals(EventTypes.FeatureCreated) && 
                            JsonSerializer.Deserialize<FeatureCreatedEvent>(ed.Data.ToArray(), null).Name.Equals(e.Name, StringComparison.InvariantCultureIgnoreCase)
                        )),
                    It.IsAny<Action<EventStoreClientOperationOptions>?>(),
                    It.IsAny<UserCredentials?>(),
                    It.IsAny<CancellationToken>()),
                Times.Once());
        }
    }
}