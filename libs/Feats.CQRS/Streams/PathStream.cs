using System;
using System.Diagnostics.CodeAnalysis;

namespace Feats.CQRS.Streams
{
    [ExcludeFromCodeCoverage]
    public sealed class PathStream: IStream
    {
        public string Name => "streams.paths";
    }
}
