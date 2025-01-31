using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventStore.Client;
using Feats.Common.Tests;
using Feats.CQRS.Events;
using Feats.CQRS.Streams;
using Feats.Domain.Aggregates;
using Feats.EventStore.Aggregates;
using Feats.EventStore.Tests.TestExtensions;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Feats.EventStore.Tests.Aggregates
{
    public abstract class FeaturesAggregateTests : TestBase
    {
        private readonly IStream _featureStream = new FeatureStream();

        [Test]
        public async Task GivenNoFeatures_WhenLoadingAggregate_ThenWeHaveEmptyFeatureList()
        {
            var reader = this.GivenIReadStreamedEvents<FeatureStream>()
                .WithEvents(System.Linq.Enumerable.Empty<IEvent>());

            var client = this.GivenIEventStoreClient()
                .WithAppendToStreamAsync(this._featureStream);

            var aggregate = this
                .GivenAggregate(reader.Object, client.Object);
            
            await aggregate
                .WithLoad()();

            aggregate.Features.Should().BeEmpty();
        }

        [Test]
        public async Task GivenUnknownEvent_WhenLoadingAggregate_ThenIgnoreIt()
        {
            var reader = this.GivenIReadStreamedEvents<FeatureStream>()
                .WithEvents(new List<IEvent>{ new NinjaEvent() });

            var client = this.GivenIEventStoreClient()
                .WithAppendToStreamAsync(this._featureStream);

            var aggregate = this
                .GivenAggregate(reader.Object, client.Object);
            
            await aggregate
                .WithLoad()();

            aggregate.Features.Should().BeEmpty();
        }
    }

    internal class NinjaEvent : IEvent
    {
        public string Type => "Akatsuki";
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

        public static Func<Task<IFeaturesAggregate>> WithLoad(this IFeaturesAggregate aggregate)
        {
            return async () => {
                await aggregate.Load();

                return aggregate;
            };
        }

        public static Func<Task> WhenPublishing(this IFeaturesAggregate aggregate, IEvent e)
        {
            return () => aggregate.Publish(e);
        }
        
        public static async Task ThenWeDontPublish(
            this Func<Task> funk,
            Mock<IEventStoreClient> mockedClient)
        {
            await funk();
            
            mockedClient.Verify(
                _ => _.AppendToStreamAsync(
                    It.IsAny<FeatureStream>(),
                    It.IsAny<StreamState>(),
                    It.IsAny<IEnumerable<EventData>>(),
                    It.IsAny<Action<EventStoreClientOperationOptions>?>(),
                    It.IsAny<UserCredentials?>(),
                    It.IsAny<CancellationToken>()),
                Times.Never());
        }
    }
}