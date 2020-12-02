
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Feats.Common.Tests;
using FluentAssertions;
using NUnit.Framework;

namespace Feats.Evaluations.Features.Metrics
{
    public class EvaluationCounterTests : TestBase
    {
        [Test]
        public async Task GivenCounter_WhenIncreasingAFeature_ThenCounterIsIncreased()
        {
            var counter = new EvaluationCounter();

            counter.Inc("one", true);
            counter.Inc("one", false);

            using var memoryStream = new MemoryStream();
            await Prometheus.Metrics.DefaultRegistry.CollectAndExportAsTextAsync(memoryStream);
            var text = Encoding.UTF8.GetString(memoryStream.ToArray());
            text.Should().NotBeEmpty();
            text.Should().Contain("{feature=\"one\",status=\"true\"} 1");
            text.Should().Contain("{feature=\"one\",status=\"false\"} 1");
        }
        
        [Test]
        public async Task GivenCounter_WhenIncreasingDistincFeatures_ThenCounterIsIncreased()
        {
            var counter = new EvaluationCounter();

            counter.Inc("thrid", true);
            counter.Inc("two", false);
            
            using var memoryStream = new MemoryStream();
            await Prometheus.Metrics.DefaultRegistry.CollectAndExportAsTextAsync(memoryStream);
            var text = Encoding.UTF8.GetString(memoryStream.ToArray());
            text.Should().NotBeEmpty();
            text.Should().Contain("{feature=\"thrid\",status=\"true\"} 1");
            text.Should().Contain("{feature=\"two\",status=\"false\"} 1");
        }
    }
}