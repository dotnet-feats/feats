using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Logging;
using Moq;

namespace Feats.Common.Tests
{
    public abstract class TestBase
    {
    }
    
    [ExcludeFromCodeCoverage]
    public static class TestsExtensions 
    {
        public static ILogger<T> GivenLogger<T>(this TestBase testClass)
        {
            return new TestLogger<T>();
        }
        
        public static ISystemClock GivenClock(this TestBase testClass)
        {
            var mock = new Mock<ISystemClock>();
            mock
                .Setup(_ => _.UtcNow)
                .Returns(new DateTimeOffset(2020, 11, 22, 12, 34, 12, 123, TimeSpan.FromHours(-4)));
            return  mock.Object;
        }

        public static async Task ThenExceptionIsThrown<T, TSomething>(this Func<Task<TSomething>> funck)
            where T : Exception
        {
            await funck.Should().ThrowAsync<T>();
        }

        public static async Task ThenExceptionIsThrown<T>(this Func<Task> funck)
            where T : Exception
        {
            await funck.Should().ThrowAsync<T>();
        }
        
        public static void ThenNoExceptionIsThrown(this Action act)
        {
            act.Should().NotThrow();
        }
                
        public static void ThenExceptionIsThrown<T>(this Action act)
            where T : Exception
        {
            act.Should().Throw<T>();
        }
        
        public static void ThenNoExceptionIsThrown<T>(this Func<T> act)
        {
            act.Should().NotThrow();
        }
                
        public static void ThenExceptionIsThrown<T, A>(this Func<A> act)
            where T : Exception
        {
            act.Should().Throw<T>();
        }
    }

    public class TestException : Exception
    {
    }
}