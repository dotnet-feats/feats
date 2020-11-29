using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Feats.CQRS.Events;
using Feats.CQRS.Streams;
using Feats.Domain.Events;
using Feats.Domain.Strategies;
using Feats.Evaluations.Tests.EventStoreSetups.TestExtensions;
using FluentAssertions;
using NUnit.Framework;

namespace Feats.Evaluations.Tests.Features
{
    public class StrategyAssignedEventTests : FeaturesAggregateTests
    {

        [Test]
        public async Task GivenNoMatchingFeatures_WhenPublishingFeaturePublijshedEvent_ThenWePublishEventThoguhIDontExist()
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

            var reader = this.GivenIReadStreamedEvents<FeatureStream>()
                .WithEvents(new List<IEvent> { created, assigned });

            var aggregate = await this
                .GivenAggregate(reader.Object)
                .WithLoad();
                            
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

            var assigned = new StrategyAssignedEvent {
                Name = "bob",
                Path = "let/me/show/you",
                StrategyName = StrategyNames.IsOn,
                Settings = "settings",
            };

            var reader = this.GivenIReadStreamedEvents<FeatureStream>()
                .WithEvents(new List<IEvent> { created, notMe, assigned });
            
            var aggregate = await this
                .GivenAggregate(reader.Object)
                .WithLoad();

            var features = aggregate.Features.ToList();

            features.Select(_ => _.Name).Should()
                .BeEquivalentTo(new List<string> { created.Name, notMe.Name });

            features.Where(_ => _.Name == assigned.Name)
                .SelectMany(_ => _.Strategies.Keys)
                .Should()
                .BeEquivalentTo(new List<string> { StrategyNames.IsOn });
        }
    }
}