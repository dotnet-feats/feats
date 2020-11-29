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
using Feats.EventStore;
using Feats.Evaluations.Tests.EventStoreSetups.TestExtensions;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Feats.Evaluations.Tests.Features
{
    public class FeatureCreatedEventTests : FeaturesAggregateTests
    {
        private readonly IStream _featureStream = new FeatureStream();

        [Test]
        public async Task GivenCreatedFeature_WhenLoading_ThenIGetFeature()
        {
            var created = new FeatureCreatedEvent {
                Name = "bob",
                Path = "let/me/show/you",
            };

            var reader = this.GivenIReadStreamedEvents<FeatureStream>()
                .WithEvents(new List<IEvent> { created });

            var aggregate = await this
                .GivenAggregate(reader.Object)
                .WithLoad();
               
            aggregate.Features.Select(_ => _.Name).Should()
                .BeEquivalentTo(new List<string> { created.Name }); 
        }        
    }
}