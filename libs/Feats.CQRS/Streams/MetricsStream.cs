using System;

namespace Feats.CQRS.Streams
{
    public sealed class MetricsStream : IStream
    {
        private readonly string _feature;

        public MetricsStream(string featureName)
        {
            this._feature = featureName;
        }

        public string Name => $"streams.metrics.{this._feature}";
    }
}
