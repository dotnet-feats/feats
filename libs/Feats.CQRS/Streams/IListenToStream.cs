using System;
using System.Threading.Tasks;

namespace Feats.CQRS.Streams
{
    public interface IListenToStream<TStream>
        where TStream: IStream
    {
        Task Listen();
    }
}
