
using System;
using System.Threading.Tasks;
using Feats.Common.Tests;
using Feats.Common.Tests.Queries;
using Feats.CQRS.Queries;
using Feats.Domain;
using Feats.Evaluations.Features;
using Feats.Evaluations.Features.Metrics;
using Feats.Evaluations.Features.Queries;
using Feats.Evaluations.Strategies;
using Feats.Evaluations.Tests.Features.Metrics.TestExtensions;
using Feats.Evaluations.Tests.Strategies.TestExtensions;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Feats.Evaluations.Tests.Features
{
    public class IsOnControllerTests : TestBase
    {
        [Test]
        public async Task GivenController_WhenCalled_ThenIReturnIsEvaluated()
        {
            var path = "path";
            var name = "name";
            var result = false;

            var counter = this.GivenIEvaluationCounter()
                .WithInc();

            var extractor = this.GivenIValuesExtractor()
                .WithEmptyExtract();

            var handler = this.GivenIHandleQuery<IsFeatureOnQuery, bool>()
                .WithResults(result);

            await this.GivenController(handler.Object, counter.Object, extractor.Object)
                .WhenCalled(path, name)
                .ThenIReturn(result)
                .AndVerifyIsCalled(path, name, result, handler, counter, extractor);
        }
    }

    public static class IsOnControllerTestsExtensions
    {
        public static IsOnController GivenController(
            this IsOnControllerTests tests,
            IHandleQuery<IsFeatureOnQuery, bool> handler,
            IEvaluationCounter evaluationCounter,
            IValuesExtractor extractor)
        {
            return new IsOnController(handler, evaluationCounter, extractor);
        }

        public static Func<Task<bool>> WhenCalled(
            this IsOnController controller,
            string path,
            string name)
        {
            return () => controller.Get(path, name);
        }

        public static async Task ThenIReturn(this Func<Task<bool>> resultsFunc, bool expectedResults)
        {
            var results = await resultsFunc();
            results.Should().Be(expectedResults);
        }
        
        public static async Task AndVerifyIsCalled(
            this Task evaluatedResultsTask,
            string path,
            string name,
            bool result,
            Mock<IHandleQuery<IsFeatureOnQuery, bool>> handler, 
            Mock<IEvaluationCounter> counter,
            Mock<IValuesExtractor> extractor)
        {
            await evaluatedResultsTask;
            handler.Verify(_ => _.Handle(It.IsAny<IsFeatureOnQuery>()), Times.Once);
            counter.Verify(_ => _.Inc(PathHelper.CombineNameAndPath(path, name), result), Times.Once);
            extractor.Verify(_ => _.Extract(), Times.Once);
        }
    }
}