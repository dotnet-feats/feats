using System.Threading.Tasks;
using Feats.CQRS.Events;

namespace Feats.CQRS
{
    public interface IAggregate
    {
        Task Load();
        
        Task Publish(IEvent e);
    }
}