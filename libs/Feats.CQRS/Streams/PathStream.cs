using System;

namespace Feats.CQRS.Streams
{
    public sealed class PathStream: IStream
    {
        public string Name => "streams.paths";
    }
}
