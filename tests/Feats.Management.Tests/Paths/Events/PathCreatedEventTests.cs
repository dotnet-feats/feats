using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Feats.CQRS.Events;
using Feats.CQRS.Streams;
using Feats.Management.Features.Events;
using Feats.Management.Tests.EventStoreSetups.TestExtensions;
using FluentAssertions;
using NUnit.Framework;

namespace Feats.Management.Tests.Paths
{
    public class PathCreatedEventTests : PathsAggregateTests
    {
        private readonly IStream _pathStream = new PathStream();

        [Test]
        public async Task GivenOtherPaths_WhenPublishingPathCreatedEvent_ThenWePublish()
        {
            var createdAlready = new PathCreatedEvent {
                FeatureAdded = "derpy",
                Path = "derpy/wants/muffins",
            };
            
            var client = this.GivenIEventStoreClient()
                .WithAppendToStreamAsync(this._pathStream);

            var reader = this.GivenIReadStreamedEvents<PathStream>()
                .WithEvents(new List<IEvent> { createdAlready });

            var created = new PathCreatedEvent {
                FeatureAdded = "bob",
                Path = "let.me",
            };

            var aggregate = await this
                .GivenAggregate(reader.Object, client.Object)
                .WithLoad();

            await aggregate
                .WhenPublishing(created)
                .ThenWePublish(client, created);

            aggregate.Paths.Select(_ => _.Name)
                .Should()
                .BeEquivalentTo(new List<string> { 
                    "derpy",
                    "derpy/wants", 
                    "derpy/wants/muffins",
                    "let", 
                    "let.me"
                });
        }

        [Test]
        public async Task GivenNoPreviousPaths_WhenPublishingPathCreatedEvent_ThenWePublish()
        {
            var client = this.GivenIEventStoreClient()
                .WithAppendToStreamAsync(this._pathStream);

            var reader = this.GivenIReadStreamedEvents<PathStream>()
                .WithEvents(Enumerable.Empty<IEvent>());

            var created = new PathCreatedEvent {
                FeatureAdded = "bob",
                Path = "let/me/show/you",
            };

            var aggregate = await this
                .GivenAggregate(reader.Object, client.Object)
                .WithLoad();

            await aggregate
                .WhenPublishing(created)
                .ThenWePublish(client, created);

            aggregate.Paths.Select(_ => _.Name)
                .Should()
                .BeEquivalentTo(new List<string> { 
                    "let",
                    "let/me", 
                    "let/me/show", 
                    "let/me/show/you"
                });
        }
        
        [Test]
        public async Task GivenNotConflictingPaths_WhenPublishingPathCreatedEvent_ThenWePublishTheFeature()
        {
            var createdAlready = new PathCreatedEvent {
                FeatureAdded = "bob",
                Path = "let/me/show/you",
            };
            
            var client = this.GivenIEventStoreClient()
                .WithAppendToStreamAsync(this._pathStream);

            var reader = this.GivenIReadStreamedEvents<PathStream>()
                .WithEvents(new List<IEvent> { createdAlready });
            
            var created = new PathCreatedEvent {
                FeatureAdded = "bob",
                Path = "let/me/show",
            };

            var aggregate = await this
                .GivenAggregate(reader.Object, client.Object)
                .WithLoad();

            await aggregate
                .WhenPublishing(created)
                .ThenWePublish(client, created);
                
            aggregate.Paths.Select(_ => _.Name).Should()
                .BeEquivalentTo(new List<string> { 
                    "let",
                    "let/me", 
                    "let/me/show", 
                    "let/me/show/you"
                });
        }
        
        
        [Test]
        public async Task GivenAdditionalPath_WhenPublishingPathCreatedEvent_ThenWePublishTheFeature()
        {
            var createdAlready = new PathCreatedEvent {
                FeatureAdded = "bob",
                Path = "let/me/show",
            };
            
            var client = this.GivenIEventStoreClient()
                .WithAppendToStreamAsync(this._pathStream);

            var reader = this.GivenIReadStreamedEvents<PathStream>()
                .WithEvents(new List<IEvent> { createdAlready });
            
            var created = new PathCreatedEvent {
                FeatureAdded = "bob",
                Path = "let/me/show/you",
            };

            var aggregate = await this
                .GivenAggregate(reader.Object, client.Object)
                .WithLoad();

            await aggregate
                .WhenPublishing(created)
                .ThenWePublish(client, created);
                
            aggregate.Paths.Select(_ => _.Name).Should()
                .BeEquivalentTo(new List<string> { 
                    "let",
                    "let/me", 
                    "let/me/show", 
                    "let/me/show/you"
                });
        }
    }
}