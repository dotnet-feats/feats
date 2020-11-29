using System.Diagnostics.CodeAnalysis;
using Feats.CQRS.Queries;
using Moq;

namespace Feats.Common.Tests
{
    [ExcludeFromCodeCoverage]
    public static class IHandleQueryExtensions 
    {
        public static Mock<IHandleQuery<TQuery, TResult>> GivenIHandleQuery<TQuery, TResult>(this TestBase tests)
            where TQuery : IQuery<TResult>
        {
            var mock = new Mock<IHandleQuery<TQuery, TResult>>();

            return mock;
        }

        public static Mock<IHandleQuery<TQuery, TResult>> WithResults<TQuery, TResult>(
            this Mock<IHandleQuery<TQuery, TResult>> mock,
            TResult result)
            where TQuery : IQuery<TResult>
        {
            mock
                .Setup(_ => _.Handle(It.IsAny<TQuery>()))
                .ReturnsAsync(result);

            return mock;
        }
    }
}