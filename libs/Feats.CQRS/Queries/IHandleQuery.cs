public namespace Feats.CQRS.Queries
{
    public interface IHandleQuery<TQuery, TResult>
        where TQuery : IQuery<TResult>
        where TResult : class
    {
        Task<TResult> Handle(TQuery query);
    }
}