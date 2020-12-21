
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Feats.Common.Tests;
using Feats.Domain;
using Feats.Domain.Aggregates;
using Feats.Management.Paths.Queries;
using Feats.Management.Tests.Paths.TestExtensions;
using FluentAssertions;
using NUnit.Framework;

namespace Feats.Management.Tests.Paths.Queries 
{
    public class GetAllPathsQueryHandlerTests : TestBase
    {
        [Test]
        public async Task GivenNoFilter_WhenRequestingAllPaths_ThenReturnAllPaths()
        {
            var expectedPaths = new List<PathAndFeatureCount>
            {
                new PathAndFeatureCount 
                {
                    Path = "derpy",
                    TotalFeatures = 3,
                },
                new PathAndFeatureCount 
                {
                    Path = "derpy/wants",
                    TotalFeatures = 2,
                },
                new PathAndFeatureCount 
                {
                    Path = "derpy/wants/muffins",
                    TotalFeatures = 1,
                },
            };

            var paths = expectedPaths.Select(e => new Path 
            {
                Name = e.Path,
                TotalFeatures = e.TotalFeatures,
            });

            var aggregate = this.GivenIPathsAggregate()
                .WithPaths(paths);

            await this.GivenHandler(aggregate.Object)
                .WhenHandlingQuery(new GetAllPathsQuery())
                .ThenReturnAllPaths(expectedPaths);
        }
        
        [Test]
        public async Task GivenFilter_WhenRequestingAllPaths_ThenReturnAllPathsTahContainTheFilter()
        {
            var expectedPaths = new List<PathAndFeatureCount>
            {
                new PathAndFeatureCount 
                {
                    Path = "derpy/wants/muffins",
                    TotalFeatures = 1,
                },
            };

            var paths = expectedPaths.Select(e => new Path 
            {
                Name = e.Path,
                TotalFeatures = e.TotalFeatures,
            });

            var aggregate = this.GivenIPathsAggregate()
                .WithPaths(paths);

            await this.GivenHandler(aggregate.Object)
                .WhenHandlingQuery(new GetAllPathsQuery() { Filter = "muffins" })
                .ThenReturnAllPaths(expectedPaths);
        }
        
        [Test]
        public async Task GivenNoPathsFound_WhenRequestingAllPaths_ThenReturnEmptyList()
        {
            var aggregate = this.GivenIPathsAggregate()
                .WithPaths(Enumerable.Empty<Path>());
            
            await this.GivenHandler(aggregate.Object)
                .WhenHandlingQuery(new GetAllPathsQuery())
                .ThenReturnAllPaths(Enumerable.Empty<PathAndFeatureCount>());
        }
    }

    public static class GetAllPathsQueryHandlerTestsExtensions
    {
        public static GetAllPathsQueryHandler GivenHandler(
            this GetAllPathsQueryHandlerTests tests,
            IPathsAggregate aggregate)
        {
            return new GetAllPathsQueryHandler(aggregate);
        }

        public static Func<Task<IEnumerable<PathAndFeatureCount>>> WhenHandlingQuery(
            this GetAllPathsQueryHandler handler, 
            GetAllPathsQuery query)
        {
            return () => handler.Handle(query);
        }

        public static async Task ThenReturnAllPaths(
            this Func<Task<IEnumerable<PathAndFeatureCount>>> resultsFunc, 
            IEnumerable<PathAndFeatureCount> expectedResults)
        {
            var results = await resultsFunc();

            results.Should().BeEquivalentTo(expectedResults);
        }
    }
}