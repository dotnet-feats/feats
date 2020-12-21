using System.Collections.Generic;
using Feats.Common.Tests;
using Feats.Domain;
using Feats.Domain.Aggregates;
using Moq;

namespace Feats.Evaluations.Tests.Features.TestExtensions
{
    public static class IFeaturesAggregateExtensions
    {
        public static Mock<IFeaturesAggregate> GivenIFeaturesAggregate(this TestBase tests)
        {
            return new Mock<IFeaturesAggregate>();
        }

        public static Mock<IFeaturesAggregate> WithFeatures(
            this Mock<IFeaturesAggregate> mock,
            IEnumerable<IFeature> features)
        {
            mock.Setup(_ => _.Features)
                .Returns(features);
    
            return mock;
        }
    }
}