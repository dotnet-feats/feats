using System;

namespace Feats.CQRS.Streams
{
    public class FeatureStream : IStream
    {
        public string Name => "streams.feature";
    }
}
