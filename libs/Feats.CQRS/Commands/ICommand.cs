
using System;

public namespace Feats.CQRS.Commands
{
    public interface ICommand
    {
        Guid CorrelationId { get; }
    }
}