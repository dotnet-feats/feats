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
using Feats.Evaluations.Features;
using Moq;
using NUnit.Framework;
using Feats.Evaluations.Tests.EventStoreSetups.TestExtensions;
using FluentAssertions;

namespace Feats.Evaluations.Tests.Features
{
    public abstract class FeaturesAggregateTests : TestBase
    {
        private readonly IStream _featureStream = new FeatureStream();

        [Test]
        public async Task GivenNoFeatures_WhenLoadingAggregate_ThenWeHaveEmptyFeatureList()
        {
            var reader = this.GivenIReadStreamedEvents<FeatureStream>()
                .WithEvents(Enumerable.Empty<IEvent>());

            var aggregate = await this
                .GivenAggregate(reader.Object)
                .WithLoad();

            aggregate.Features.Should().BeEmpty();
        }

        [Test]
        public async Task GivenUnknownEvent_WhenLoadingAggregate_ThenIgnoreIt()
        {
            var reader = this.GivenIReadStreamedEvents<FeatureStream>()
                .WithEvents(new List<IEvent>{ new NinjaEvent() });

            var aggregate = await this
                .GivenAggregate(reader.Object)
                .WithLoad();

            aggregate.Features.Should().BeEmpty();
        }
    }

    internal class NinjaEvent : IEvent
    {
        public string Type => "Kakashi";
    }

    public static class FeaturesAggregateTestsExtensions
    {
        public static IFeaturesAggregate GivenAggregate(
            this FeaturesAggregateTests tests, 
            IReadStreamedEvents<FeatureStream> reader)
        {
            return new FeaturesAggregate(
                tests.GivenLogger<FeaturesAggregate>(), 
                reader);
        }

        public static async Task<IFeaturesAggregate> WithLoad(this IFeaturesAggregate aggregate)
        {
            await aggregate.Load();

            return aggregate;
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