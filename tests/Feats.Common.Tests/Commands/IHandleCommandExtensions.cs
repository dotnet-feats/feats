using System;
using System.Diagnostics.CodeAnalysis;
using Feats.CQRS.Commands;
using Moq;

namespace Feats.Common.Tests
{
    [ExcludeFromCodeCoverage]
    public static class IHandleCommandExtensions 
    {
        public static Mock<IHandleCommand<T>> GivenCommandHandler<T>(this TestBase testClass)
            where T : ICommand
        {
            var mock = new Mock<IHandleCommand<T>>();
            
            return mock;
        }

        public static Mock<IHandleCommand<T>> WithHandling<T>(this Mock<IHandleCommand<T>> mock)
            where T : ICommand
        {
            mock
                .Setup(_ => _.Handle(It.IsAny<T>()));
            
            return mock;
        }

        public static Mock<IHandleCommand<T>> WithException<T, TException>(this Mock<IHandleCommand<T>> mock)
            where T : ICommand
            where TException : Exception, new()
        {
            mock
                .Setup(_ => _.Handle(It.IsAny<T>()))
                .ThrowsAsync(new TException());

            return mock;
        }
    }
}