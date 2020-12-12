using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Feats.Common.Tests;
using Feats.Common.Tests.Queries;
using Feats.CQRS.Queries;
using Feats.Domain;
using Feats.Management.Features;
using Feats.Management.Features.Exceptions;
using Feats.Management.Features.Queries;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Feats.Management.Tests.Features
{
    public class GetFeatureControllerTests : TestBase
    {
        [Test]
        public async Task GivenEmptyPathFeature_WhenQueryByNameMatches_ThenWeReturnTheResults()
        {
            var path = string.Empty;
            var name = "name";
            var expectedResults = new FeatureAndStrategyConfiguration
            {
                Path = string.Empty,
                Feature = name,
                State = FeatureState.Published,
                Strategies = new Dictionary<string, string>
                {
                    { "one", "one" },
                },
            };

            var handler = this
                .GivenIHandleQuery<GetFeatureQuery, FeatureAndStrategyConfiguration>()
                .WithResults(expectedResults);

            await this
                .GivenController(handler.Object)
                .WhenProcessingQuery(path, name)
                .ThenWeReturnFeatureAndStrategies(expectedResults, handler);
        }

        [Test]
        public async Task GivenAFilterQueryString_WhenQueryReturnsResults_ThenWeReturnTheResults()
        {
            var expectedResults = new FeatureAndStrategyConfiguration
            {
                Path = "bob/painting",
                Feature = "ross&friends"
            };

            var path = WebUtility.UrlEncode("bob/painting");
            var name = WebUtility.UrlEncode("ross&friends");

            var handler = this
                .GivenIHandleQuery<GetFeatureQuery, FeatureAndStrategyConfiguration>()
                .WithResults(expectedResults);

            await this
                .GivenController(handler.Object)
                .WhenProcessingQuery(path, name)
                .ThenWeReturnFeatureAndStrategies(expectedResults, handler);
        }

        [Test]
        public async Task GivenRequest_WhenQueryReturnsResultsThrows_ThenWeThrow()
        {
            var handler = this
                .GivenIHandleQuery<GetFeatureQuery, FeatureAndStrategyConfiguration>()
                .WithException<GetFeatureQuery, FeatureAndStrategyConfiguration, FeatureNotFoundException>();

            await this
                .GivenController(handler.Object)
                .WhenProcessingQuery("bob", "ross")
                .ThenExceptionIsThrown<FeatureNotFoundException, FeatureAndStrategyConfiguration>();
        }
    }

    public static class GetFeatureControllerTestsExtensions
    {
        public static GetFeatureController GivenController(
            this GetFeatureControllerTests tests,
            IHandleQuery<GetFeatureQuery, FeatureAndStrategyConfiguration> handler)
        {
            return new GetFeatureController(handler);
        }

        public static Func<Task<FeatureAndStrategyConfiguration>> WhenProcessingQuery(
            this GetFeatureController controller,
            string path,
            string name)
        {
            return () => controller.Get(path, name);
        }

        public static async Task ThenWeReturnFeatureAndStrategies(
            this Func<Task<FeatureAndStrategyConfiguration>> processingFunc,
            FeatureAndStrategyConfiguration expectedResult,
            Mock<IHandleQuery<GetFeatureQuery, FeatureAndStrategyConfiguration>> handler)
        {
            var results = await processingFunc();
            results.Should().BeEquivalentTo(expectedResult);
            handler.Verify(_ => _.Handle(It.Is<GetFeatureQuery>(q => 
                q.Name.Equals(expectedResult.Feature) && 
                q.Path.Equals(expectedResult.Path))), Times.Once);
        }
    }
}