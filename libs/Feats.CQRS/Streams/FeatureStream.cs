using System;
using System.Diagnostics.CodeAnalysis;

namespace Feats.CQRS.Streams
{
    [ExcludeFromCodeCoverage]
    public sealed class FeatureStream : IStream
    {
        public string Name => "streams.feature";
    }
}
