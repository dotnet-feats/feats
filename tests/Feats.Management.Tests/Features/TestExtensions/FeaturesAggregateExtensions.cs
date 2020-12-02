using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Feats.Common.Tests;
using Feats.CQRS.Events;
using Feats.Domain;
using Feats.Management.Features;
using Moq;

namespace Feats.Management.Tests.Features.TestExtensions
{
    public static class IFeaturesAggregateExtensions
    {
        public static Mock<IFeaturesAggregate> GivenIFeaturesAggregate(this TestBase testClass)
        {
            return new Mock<IFeaturesAggregate>();
        }

        public static Mock<IFeaturesAggregate> WithPublishing(
            this Mock<IFeaturesAggregate>  mock)
        {
            mock
                .Setup(_ => _.Publish(It.IsAny<IEvent>()))
                .Returns(Task.CompletedTask);

            return mock;
        }

        public static Mock<IFeaturesAggregate> WithFeatures(
            this Mock<IFeaturesAggregate>  mock, 
            IEnumerable<IFeature> features)
        {
            mock.Setup(_ => _.Features)
                .Returns(features);

            return mock;
        }

        public static Mock<IFeaturesAggregate> WithPublishingThrows<TException>(
            this Mock<IFeaturesAggregate>  mock)
            where TException : Exception, new ()
        {
            mock.Setup(_ => _.Publish(It.IsAny<IEvent>()))
                .ThrowsAsync(new TException());

            return mock;
        }

        public static IFeature GivenRandomFeature(this TestBase testClass)
        {
            return new Feature
            {
                Name = Guid.NewGuid().ToString(),
                Path = $"{Guid.NewGuid().ToString()}/moo",
            };
        }
    }
}