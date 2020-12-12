using System;
using System.Collections.Generic;
using Feats.CQRS.Events;

namespace Feats.CQRS.Streams
{
    public interface IReadStreamedEvents<TStream>
        where TStream: IStream
    {
        IAsyncEnumerable<IEvent> Read();
    }
}
