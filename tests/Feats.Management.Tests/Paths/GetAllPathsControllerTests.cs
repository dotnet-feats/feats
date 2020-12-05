using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Feats.Common.Tests;
using Feats.CQRS.Queries;
using Feats.Management.Features;
using Feats.Management.Paths.Queries;
using FluentAssertions;
using NUnit.Framework;

namespace Feats.Management.Tests.Paths
{
    public class GetAllPathsControllerTests : TestBase
    {
        [Test]
        public async Task GivenEmptyFilterQueryString_WhenQueryReturnsResults_ThenWeReturnTheResults()
        {
            var filter = string.Empty;
            var expectedResults = new List<PathAndFeatureCount>
            {
                new PathAndFeatureCount 
                {
                    Path = "bob",
                    TotalFeatures = 4,
                },
                
                new PathAndFeatureCount 
                {
                    Path = "bob",
                    TotalFeatures = 1,
                },
            };

            var handler = this
                .GivenIHandleQuery<GetAllPathsQuery, IEnumerable<PathAndFeatureCount>>()
                .WithResults(expectedResults);

            await this
                .GivenController(handler.Object)
                .WhenProcessingQuery(filter)
                .ThenWeReturnPathsAndCounts(expectedResults);
        }

        [Test]
        public async Task GivenAFilterQueryString_WhenQueryReturnsResults_ThenWeReturnTheResults()
        {
            var filter = string.Empty;
            var expectedResults = new List<PathAndFeatureCount>
            {
                new PathAndFeatureCount 
                {
                    Path = "bob",
                    TotalFeatures = 4,
                },
            };

            var handler = this
                .GivenIHandleQuery<GetAllPathsQuery, IEnumerable<PathAndFeatureCount>>()
                .WithResults(expectedResults);

            await this
                .GivenController(handler.Object)
                .WhenProcessingQuery(filter)
                .ThenWeReturnPathsAndCounts(expectedResults);
        }

        [Test]
        public async Task GivenAFilterQueryString_WhenQueryReturnsNoResults_ThenWeReturnNoResults()
        {
            var filter = "🦄";
            var expectedResults = Enumerable.Empty<PathAndFeatureCount>();

            var handler = this
                .GivenIHandleQuery<GetAllPathsQuery, IEnumerable<PathAndFeatureCount>>()
                .WithResults(expectedResults);

            await this
                .GivenController(handler.Object)
                .WhenProcessingQuery(filter)
                .ThenWeReturnPathsAndCounts(expectedResults);
        }
        
        [Test]
        public async Task GivenRequest_WhenQueryReturnsResultsThrows_ThenWeThrow()
        {
            var filter = "a";
            var handler = this
                .GivenIHandleQuery<GetAllPathsQuery, IEnumerable<PathAndFeatureCount>>()
                .WithException<GetAllPathsQuery, IEnumerable<PathAndFeatureCount>, TestException>();

            await this
                .GivenController(handler.Object)
                .WhenProcessingQuery(filter)
                .ThenExceptionIsThrown<TestException, IEnumerable<PathAndFeatureCount>>();
        }
    }

    public static class GetAllPathsControllerTestsExtensions
    {
        public static GetAllPathsController GivenController(
            this GetAllPathsControllerTests tests, 
            IHandleQuery<GetAllPathsQuery, IEnumerable<PathAndFeatureCount>> handler)
        {
            return new GetAllPathsController(handler);
        }

        public static Func<Task<IEnumerable<PathAndFeatureCount>>> WhenProcessingQuery(
            this GetAllPathsController controller,
            string filter)
        {
            return () => controller.Get(filter);
        }

        public static async Task ThenWeReturnPathsAndCounts(
            this Func<Task<IEnumerable<PathAndFeatureCount>>> processingFunc,
            IEnumerable<PathAndFeatureCount> expectedResults)
        {
            var results = await processingFunc();
            results.Should().BeEquivalentTo(expectedResults);
        }
    }
}