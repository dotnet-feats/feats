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
using Feats.Evaluations.Tests.EventStoreSetups.TestExtensions;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Feats.Evaluations.Tests.Features
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

            var aggregate = await this
                .GivenAggregate(reader.Object)
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

            var aggregate = await this
                .GivenAggregate(reader.Object)
                .WithLoad();

            var features = aggregate.Features.ToList();

            features.Should().Contain(_ => _.Name == created.Name);
            features.Should().NotContain(_ => _.Name == published.Name);
        }

        [Test]
        public async Task GivenAMatchingFeature_WhenLoading_ThenWeGetAPublishedFeature()
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

            var aggregate = await this
                .GivenAggregate(reader.Object)
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
    }
}