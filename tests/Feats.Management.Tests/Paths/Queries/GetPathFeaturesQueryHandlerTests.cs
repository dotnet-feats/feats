
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Feats.Common.Tests;
using Feats.Domain;
using Feats.EventStore.Aggregates;
using Feats.Management.Paths.Queries;
using Feats.Management.Tests.Features.TestExtensions;
using FluentAssertions;
using Microsoft.Extensions.Internal;
using NUnit.Framework;

namespace Feats.Management.Tests.Paths.Queries 
{
    public class GetPathFeaturesQueryHandlerTests : TestBase
    {
        private readonly ISystemClock _clock;

        public GetPathFeaturesQueryHandlerTests()
        {
            this._clock = this.GivenClock();
        }

        [Test]
        public async Task GivenAQuery_WhenQueryingAnEmptyPath_ThenReturnAllFeatures()
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
                .WhenHandlingQuery(new GetPathFeaturesQuery())
                .ThenReturnFeatures(features);
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
                .WhenHandlingQuery(new GetPathFeaturesQuery() { Path = "derpy/wants" })
                .ThenReturnFeatures(features.Skip(1));
        }
        
        [Test]
        public async Task GivenUnmatchedFilter_WhenQueryFeaturesInPath_ThenReturnEmptyList()
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
                .WhenHandlingQuery(new GetPathFeaturesQuery() { Path = "il/va/faire/tout/noir" })
                .ThenReturnFeatures(Enumerable.Empty<Feature>());
        }

        [Test]
        public async Task GivenNoPathsFound_WhenQueryFeaturesInPath_ThenReturnEmptyList()
        {
            var aggregate = this.GivenIFeaturesAggregate()
                .WithFeatures(Enumerable.Empty<Feature>());
            
            await this.GivenHandler(aggregate.Object)
                .WhenHandlingQuery(new GetPathFeaturesQuery())
                .ThenReturnFeatures(Enumerable.Empty<Feature>());
        }
    }

    public static class GetPathFeaturesQueryHandlerTestsExtensions
    {
        public static GetPathFeaturesQueryHandler GivenHandler(
            this GetPathFeaturesQueryHandlerTests tests,
            IFeaturesAggregate aggregate)
        {
            return new GetPathFeaturesQueryHandler(aggregate);
        }

        public static Func<Task<IEnumerable<FeatureAndStrategy>>> WhenHandlingQuery(
            this GetPathFeaturesQueryHandler handler, 
            GetPathFeaturesQuery query)
        {
            return () => handler.Handle(query);
        }

        public static async Task ThenReturnFeatures(
            this Func<Task<IEnumerable<FeatureAndStrategy>>> resultsFunc, 
            IEnumerable<Feature> features)
        {
            var results = await resultsFunc();

            results.Should().BeEquivalentTo(features
                .Select(_ => new FeatureAndStrategy
                    {
                        Feature = _.Name,
                        Path = _.Path,
                        State = _.State,
                        StrategyNames = _.Strategies.Select(_ => _.Key),
                        CreatedBy = _.CreatedBy,
                        CreatedOn = _.CreatedOn,
                        UpdatedOn = _.UpdatedOn,
                    }));
        }
    }
}