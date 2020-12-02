using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Feats.Common.Tests;
using Feats.CQRS.Events;
using Feats.Domain;
using Feats.Management.Paths;
using Moq;

namespace Feats.Management.Tests.Paths.TestExtensions
{
    public static class IPathsAggregateExtensions
    {
        public static Mock<IPathsAggregate> GivenIPathsAggregate(this TestBase testClass)
        {
            return new Mock<IPathsAggregate>();
        }

        public static Mock<IPathsAggregate> WithPaths(
            this Mock<IPathsAggregate>  mock, 
            IEnumerable<IPath> paths)
        {
            mock.Setup(_ => _.Paths)
                .Returns(paths);

            return mock;
        }

        public static Mock<IPathsAggregate> WithPublishing(
            this Mock<IPathsAggregate>  mock)
        {
            mock
                .Setup(_ => _.Publish(It.IsAny<IEvent>()))
                .Returns(Task.CompletedTask);

            return mock;
        }

        public static Mock<IPathsAggregate> WithPublishingThrows<TException>(
            this Mock<IPathsAggregate>  mock)
            where TException : Exception, new ()
        {
            mock.Setup(_ => _.Publish(It.IsAny<IEvent>()))
                .ThrowsAsync(new TException());

            return mock;
        }

        public static IPath GivenRandomPath(this TestBase testClass)
        {
            var random = new Random();
            return new Path
            {
                Name = $"{Guid.NewGuid().ToString()}/moo",
                FeatureNames = Enumerable.Range(0, random.Next(5)).Select(_ => Guid.NewGuid().ToString()),
            };
        }
    }
}