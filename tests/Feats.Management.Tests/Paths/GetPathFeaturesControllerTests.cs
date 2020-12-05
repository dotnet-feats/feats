using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Feats.Common.Tests;
using Feats.CQRS.Queries;
using Feats.Domain;
using Feats.Management.Paths;
using Feats.Management.Paths.Queries;
using FluentAssertions;
using NUnit.Framework;

namespace Feats.Management.Tests.Paths
{
    public class GetPathFeaturesControllerTests : TestBase
    {
        [Test]
        public async Task GivenEmptyFilterQueryString_WhenQueryReturnsResults_ThenWeReturnTheResults()
        {
            var path = string.Empty;
            var expectedResults = new List<FeatureAndStrategy>
            {
                new FeatureAndStrategy 
                {
                    Path = "bob",
                    Feature = "ross",
                },
                
                new FeatureAndStrategy 
                {
                    Path = "bob/is",
                    Feature = "ross",
                    State = FeatureState.Published,
                    StrategyNames = new List<string> { "one", "two" },
                },
            };

            var handler = this
                .GivenIHandleQuery<GetPathFeaturesQuery, IEnumerable<FeatureAndStrategy>>()
                .WithResults(expectedResults);

            await this
                .GivenController(handler.Object)
                .WhenProcessingQuery(path)
                .ThenWeReturnFeatureAndStrategies(expectedResults);
        }

        [Test]
        public async Task GivenAFilterQueryString_WhenQueryReturnsResults_ThenWeReturnTheResults()
        {
            var filter = string.Empty;
            var expectedResults = new List<FeatureAndStrategy>
            {
                new FeatureAndStrategy 
                {
                    Path = "bob/painting",
                    Feature = "ross"
                },
            };

            var path = WebUtility.UrlEncode("bob/painting");

            var handler = this
                .GivenIHandleQuery<GetPathFeaturesQuery, IEnumerable<FeatureAndStrategy>>()
                .WithResults(expectedResults);

            await this
                .GivenController(handler.Object)
                .WhenProcessingQuery(path)
                .ThenWeReturnFeatureAndStrategies(expectedResults);
        }

        [Test]
        public async Task GivenAFilterQueryString_WhenQueryReturnsNoResults_ThenWeReturnNoResults()
        {
            var path = string.Empty;
            var expectedResults = Enumerable.Empty<FeatureAndStrategy>();

            var handler = this
                .GivenIHandleQuery<GetPathFeaturesQuery, IEnumerable<FeatureAndStrategy>>()
                .WithResults(expectedResults);

            await this
                .GivenController(handler.Object)
                .WhenProcessingQuery(path)
                .ThenWeReturnFeatureAndStrategies(expectedResults);
        }
        
        [Test]
        public async Task GivenRequest_WhenQueryReturnsResultsThrows_ThenWeThrow()
        {
            var handler = this
                .GivenIHandleQuery<GetPathFeaturesQuery, IEnumerable<FeatureAndStrategy>>()
                .WithException<GetPathFeaturesQuery, IEnumerable<FeatureAndStrategy>, TestException>();

            await this
                .GivenController(handler.Object)
                .WhenProcessingQuery("bob")
                .ThenExceptionIsThrown<TestException, IEnumerable<FeatureAndStrategy>>();
        }
    }

    public static class GetPathFeaturesControllerTestsExtensions
    {
        public static GetPathFeaturesController GivenController(
            this GetPathFeaturesControllerTests tests, 
            IHandleQuery<GetPathFeaturesQuery, IEnumerable<FeatureAndStrategy>> handler)
        {
            return new GetPathFeaturesController(handler);
        }

        public static Func<Task<IEnumerable<FeatureAndStrategy>>> WhenProcessingQuery(
            this GetPathFeaturesController controller,
            string path)
        {
            return () => controller.Get(path);
        }

        public static async Task ThenWeReturnFeatureAndStrategies(
            this Func<Task<IEnumerable<FeatureAndStrategy>>> processingFunc,
            IEnumerable<FeatureAndStrategy> expectedResults)
        {
            var results = await processingFunc();
            results.Should().BeEquivalentTo(expectedResults);
        }
    }
}