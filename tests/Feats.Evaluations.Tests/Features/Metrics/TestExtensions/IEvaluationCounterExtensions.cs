using Feats.Common.Tests;
using Feats.Evaluations.Features.Metrics;
using Moq;

namespace Feats.Evaluations.Tests.Features.Metrics.TestExtensions
{
    public static class IEvaluationCounterExtensions
    {
        public static Mock<IEvaluationCounter> GivenIEvaluationCounter(this TestBase tests)
        {
            return new Mock<IEvaluationCounter>();
        }

        public static Mock<IEvaluationCounter> WithInc(this Mock<IEvaluationCounter> mock)
        {
            mock.Setup(_ => _.Inc(It.IsAny<string>(), It.IsAny<bool>()));

            return mock;
        }
    }
}