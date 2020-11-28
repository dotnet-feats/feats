using System;

namespace Feats.CQRS.Streams
{
    public class MetricsStream : IStream
    {
        public string Name => "streams.metrics";
    }
}
