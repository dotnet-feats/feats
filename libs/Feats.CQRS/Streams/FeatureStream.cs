using System;

namespace Feats.CQRS.Streams
{
    public sealed class FeatureStream : IStream
    {
        public string Name => "streams.feature";
    }
}
