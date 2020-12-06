
using System;
using System.Collections.Generic;
using Feats.Common.Tests;
using Feats.Evaluations.Strategies;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Moq;
using NUnit.Framework;

namespace Feats.Evaluations.Tests.Strategies
{
    public class ValuesExtractorTests : TestBase
    {
        [Test]
        public void GivenAnHttpContextWithHeaders_WhenExtracting_ThenIGetAllHeaders()
        {
            var values = new Dictionary<string, StringValues> 
            {
                { "a", "b" },
                { "list", "42" },
            };

            var headers = this.GivenHeaders(values);
            var context = this.GivenContext()
                .WithHeaders(headers);
            var accessor = this.GivenIHttpContextAccessor()
                .WithHttpContext(context.Object);

            this.GivenExtractor(accessor.Object)
                .WhenExtracting()
                .ThenIHaveMyHeaders(values);
        }
        
        [Test]
        public void GivenAnHttpContextEmptyOfHeaders_WhenExtracting_ThenIHaveEmptyHeaders()
        {
            var values = new Dictionary<string, StringValues>();

            var headers = this.GivenHeaders(values);
            var context = this.GivenContext()
                .WithHeaders(headers);
            var accessor = this.GivenIHttpContextAccessor()
                .WithHttpContext(context.Object);

            this.GivenExtractor(accessor.Object)
                .WhenExtracting()
                .ThenIHaveEmptyHeaders(values);
        }
    }
    public static class ValuesExtractorTestsExtensions
    {
        public static IHeaderDictionary GivenHeaders(
            this ValuesExtractorTests tests,
            Dictionary<string, StringValues> values)
        {
            return new HeaderDictionary(values);
        }

        public static IHeaderDictionary GivenEmptyHeaders(
            this ValuesExtractorTests tests)
        {
            return new HeaderDictionary(new Dictionary<string, StringValues>());
        }

        public static Mock<HttpContext> GivenContext(this ValuesExtractorTests tests)
        {
            return new Mock<HttpContext>();
        }

        public static Mock<IHttpContextAccessor> GivenIHttpContextAccessor(
            this ValuesExtractorTests tests)
        {
            return new Mock<IHttpContextAccessor>();
        }

        public static Mock<IHttpContextAccessor> WithHttpContext(
            this Mock<IHttpContextAccessor> mock,
            HttpContext context)
        {
            mock
                .Setup(_ => _.HttpContext)
                .Returns(context);

            return mock;
        }
        
        public static Mock<HttpContext> WithHeaders(
            this Mock<HttpContext> mock,
            IHeaderDictionary headers)
        {
            var request = new Mock<HttpRequest>();

            request
                .Setup(_ => _.Headers)
                .Returns(headers);
            mock.Setup(_ => _.Request)
                .Returns(request.Object);

            return mock;
        }

        public static IValuesExtractor GivenExtractor(
            this ValuesExtractorTests tests,
            IHttpContextAccessor accessor)
        {
            return new ValuesExtractor(accessor);
        }
        
        public static Func<IDictionary<string, string>> WhenExtracting(
            this IValuesExtractor extractor)
        {
            return () => extractor.Extract();
        }
        
        public static void ThenIHaveMyHeaders(
            this Func<IDictionary<string, string>> func,
            Dictionary<string, StringValues> values)
        {
            var results = func();
            foreach(var (key, items) in results)
            {
                values.Keys.Should().Contain(key);
                var exectedValues = values[key];

                exectedValues.ToString().Should().Be(items);
            }
        }
        
        public static void ThenIHaveEmptyHeaders(
            this Func<IDictionary<string, string>> func,
            Dictionary<string, StringValues> values)
        {
            var results = func();
            results.Should().BeEmpty();
        }
    }
}
