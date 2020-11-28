using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Feats.Common.Tests;
using Feats.CQRS.Events;
using Feats.CQRS.Streams;
using Feats.Management.Features.Events;
using Feats.Management.Features.Exceptions;
using Feats.Management.Tests.EventStoreSetups.TestExtensions;
using FluentAssertions;
using NUnit.Framework;

namespace Feats.Management.Tests.Features
{
    public class FeatureCreatedEventTests : FeaturesAggregateTests
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
}