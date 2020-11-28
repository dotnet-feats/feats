using System;

namespace Feats.CQRS.Events
{
    public interface IEvent
    {
        string Type { get; }
    }
}
