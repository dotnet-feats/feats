using System.Threading.Tasks;

namespace Feats.CQRS.Queries
{
    public interface IHandleQuery<TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        Task<TResult> Handle(TQuery query);
    }
}