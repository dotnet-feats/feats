using System;

namespace Feats.CQRS.Streams
{
    public class PathStream: IStream
    {
        public string Name => "streams.paths";
    }
}
