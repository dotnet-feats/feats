using System.Collections.Generic;
using Feats.Common.Tests;
using Feats.Evaluations.Strategies;
using Moq;

namespace Feats.Evaluations.Tests.Strategies.TestExtensions
{
    public static class IStrategyEvaluatorFactoryExtensions
    {
        public static Mock<IStrategyEvaluatorFactory> GivenIStrategyEvaluatorFactory(this TestBase tests)
        {
            return new Mock<IStrategyEvaluatorFactory>();
        }
        
        public static Mock<IStrategyEvaluatorFactory> WithOn(this Mock<IStrategyEvaluatorFactory> mock)
        {
            mock
                .Setup(_ => _.IsOn(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<IDictionary<string, string>>()))
                .ReturnsAsync(true);

            return mock;
        }
        
        public static Mock<IStrategyEvaluatorFactory> With(
            this Mock<IStrategyEvaluatorFactory> mock,
            params bool[] calls)
        {
            var seq = mock
                .SetupSequence(_ => _.IsOn(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<IDictionary<string, string>>()));

            foreach(var call in calls)
            {
                seq.ReturnsAsync(call);
            }

            return mock;
        }
        
        public static Mock<IStrategyEvaluatorFactory> WithOff(this Mock<IStrategyEvaluatorFactory> mock)
        {
            mock
                .Setup(_ => _.IsOn(
                    It.IsAny<string>(), 
                    It.IsAny<string>(),
                    It.IsAny<IDictionary<string, string>>()))
                .ReturnsAsync(false);

            return mock;
        }
    }
}