
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Feats.Common.Tests;
using Feats.Domain;
using Feats.Management.Features;
using Feats.Management.Features.Exceptions;
using Feats.Management.Features.Queries;
using Feats.Management.Tests.Features.TestExtensions;
using FluentAssertions;
using Microsoft.Extensions.Internal;
using NUnit.Framework;

namespace Feats.Management.Tests.Features.Queries 
{
    public class GetFeatureQueryHandlerTests : TestBase
    {
        private readonly ISystemClock _clock;

        public GetFeatureQueryHandlerTests()
        {
            this._clock = this.GivenClock();
        }

        [Test]
        public async Task GivenAQueryWithSinglePath_WhenQuerying_ThenReturnMatchingFeature()
        {
            var features = new List<Feature> 
            {
                new Feature 
                {
                    Name = "hooves",
                    Path = "derpy",
                    CreatedBy = "someone",
                    CreatedOn = this._clock.UtcNow,
                },
                new Feature 
                {
                    Name = "published",
                    Path = "derpy/wants",
                    CreatedBy = "someone",
                    CreatedOn = this._clock.UtcNow,
                    UpdatedOn = this._clock.UtcNow,
                    State = FeatureState.Published,
                    Strategies = new Dictionary<string, string> 
                    {
                        {"test", "test"},
                        {"test2", "test"}
                    },
                },
            };

            var aggregate = this.GivenIFeaturesAggregate()
                .WithFeatures(features);

            await this.GivenHandler(aggregate.Object)
                .WhenHandlingQuery(new GetFeatureQuery()
                {
                    Path = "derpy",
                    Name = "hooves",
                })
                .ThenReturnFeature(features.First());
        }

        [Test]
        public async Task GivenAQuery_WhenQueryingAnEmptyPath_ThenReturnMatchingFeature()
        {
            var features = new List<Feature> 
            {
                new Feature 
                {
                    Name = "hooves",
                    Path = string.Empty,
                    CreatedBy = "someone",
                    CreatedOn = this._clock.UtcNow,
                },
                new Feature 
                {
                    Name = "published",
                    Path = "derpy/wants",
                    CreatedBy = "someone",
                    CreatedOn = this._clock.UtcNow,
                    UpdatedOn = this._clock.UtcNow,
                    State = FeatureState.Published,
                    Strategies = new Dictionary<string, string> 
                    {
                        {"test", "test"},
                        {"test2", "test"}
                    },
                },
            };

            var aggregate = this.GivenIFeaturesAggregate()
                .WithFeatures(features);

            await this.GivenHandler(aggregate.Object)
                .WhenHandlingQuery(new GetFeatureQuery()
                {
                    Path = string.Empty,
                    Name = "hooves",
                })
                .ThenReturnFeature(features.First());
        }
        
        [Test]
        public async Task GivenPath_WhenQueryFeaturesInPath_ThenReturnAllFeaturesWithinPath()
        {
            var features = new List<Feature> 
            {
                new Feature 
                {
                    Name = "hooves",
                    Path = "derpy",
                    CreatedBy = "someone",
                    CreatedOn = this._clock.UtcNow,
                },
                new Feature 
                {
                    Name = "published",
                    Path = "derpy/wants",
                    CreatedBy = "someone",
                    CreatedOn = this._clock.UtcNow,
                    UpdatedOn = this._clock.UtcNow,
                    State = FeatureState.Published,
                    Strategies = new Dictionary<string, string> 
                    {
                        {"test", "test"},
                        {"test2", "test"}
                    },
                },
            };
            
            var aggregate = this.GivenIFeaturesAggregate()
                .WithFeatures(features);

            await this.GivenHandler(aggregate.Object)
                .WhenHandlingQuery(new GetFeatureQuery() { Path = "derpy/wants", Name = "published" })
                .ThenReturnFeature(features.Last());
        }
        
        [Test]
        public async Task GivenUnmatchedFilter_WhenQueryFeature_ThenFeatureNotFoundIsThrown()
        {
            var features = new List<Feature> 
            {
                new Feature 
                {
                    Name = "hooves",
                    Path = "derpy",
                    CreatedBy = "someone",
                    CreatedOn = this._clock.UtcNow,
                },
            };
            
            var aggregate = this.GivenIFeaturesAggregate()
                .WithFeatures(features);

            await this.GivenHandler(aggregate.Object)
                .WhenHandlingQuery(new GetFeatureQuery() { Path = "il/va/faire/tout/noir" })
                .ThenExceptionIsThrown<FeatureNotFoundException>();
        }

        [Test]
        public async Task GivenNoPathsFound_WhenQueryFeature_ThenFeatureNotFoundsThrown()
        {
            var aggregate = this.GivenIFeaturesAggregate()
                .WithFeatures(Enumerable.Empty<Feature>());
            
            await this.GivenHandler(aggregate.Object)
                .WhenHandlingQuery(new GetFeatureQuery() 
                {
                    Name = "hello",
                })
                .ThenExceptionIsThrown<FeatureNotFoundException>();
        }

        [Test]
        public async Task GivenNoPathNoName_WhenQueryFeature_ThenFeatureNotFoundsThrown()
        {
            var aggregate = this.GivenIFeaturesAggregate()
                .WithFeatures(Enumerable.Empty<Feature>());
            
            await this.GivenHandler(aggregate.Object)
                .WhenHandlingQuery(new GetFeatureQuery())
                .ThenExceptionIsThrown<FeatureNotFoundException>();
        }
    }

    public static class GetFeatureQueryHandlerTestsExtensions
    {
        public static GetFeatureQueryHandler GivenHandler(
            this GetFeatureQueryHandlerTests tests,
            IFeaturesAggregate aggregate)
        {
            return new GetFeatureQueryHandler(aggregate);
        }

        public static Func<Task<FeatureAndStrategyConfiguration>> WhenHandlingQuery(
            this GetFeatureQueryHandler handler, 
            GetFeatureQuery query)
        {
            return () => handler.Handle(query);
        }

        public static async Task ThenReturnFeature(
            this Func<Task<FeatureAndStrategyConfiguration>> resultsFunc, 
            Feature feature)
        {
            var result = await resultsFunc();
            result.Path.Should().Be(feature.Path);
            result.Feature.Should().Be(feature.Name);
            result.State.Should().Be(feature.State);
            result.Strategies.Should().BeEquivalentTo(feature.Strategies);
            result.CreatedBy.Should().Be(feature.CreatedBy);
            result.CreatedOn.Should().Be(feature.CreatedOn);
            result.UpdatedOn.Should().Be(feature.UpdatedOn);
        }
    }
}