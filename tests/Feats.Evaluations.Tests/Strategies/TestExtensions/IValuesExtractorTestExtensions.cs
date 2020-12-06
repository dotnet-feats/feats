using System.Collections.Generic;
using System.Threading.Tasks;
using Feats.Common.Tests;
using Feats.Evaluations.Strategies;
using Moq;

namespace Feats.Evaluations.Tests.Strategies.TestExtensions
{
    public static class IValuesExtractorTestExtensions
    {
        public static Mock<IValuesExtractor> GivenIValuesExtractor(this TestBase tests)
        {
            return new Mock<IValuesExtractor>();
        }
        
        public static Mock<IValuesExtractor> WithExtract(
            this Mock<IValuesExtractor> mock,
            IDictionary<string, string> values)
        {
            mock
                .Setup(_ => _.Extract())
                .Returns(values);

            return mock;
        }
        
        public static Mock<IValuesExtractor> WithEmptyExtract(this Mock<IValuesExtractor> mock)
        {
            mock
                .Setup(_ => _.Extract())
                .Returns(new Dictionary<string, string>());

            return mock;
        }
    }
}