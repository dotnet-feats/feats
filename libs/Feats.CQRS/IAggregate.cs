using System.Threading.Tasks;
using Feats.CQRS.Events;

namespace Feats.CQRS
{
    public interface IAggregate : IReadonlyAggregate
    {        
        Task Publish(IEvent e);
    }
    
    public interface IReadonlyAggregate
    {
        Task Load();
    }
}