public namespace Feats.CQRS.Streams
{
    public interface IStream
    {
        string Name { get; }
        
        long Position { get; }
    }
}