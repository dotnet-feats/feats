using System;
using System.Threading.Tasks;

namespace Feats.CQRS.Events
{
    public interface IHandleEvent
    {
        Task Handle(string eventTypeName, string jsonEvent);
    }
}
