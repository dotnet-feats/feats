using System.Collections.Generic;
using Feats.CQRS;

namespace Feats.Domain.Aggregates
{
    public interface IPathsAggregate : IAggregate
    {
        IEnumerable<IPath> Paths { get; }
    }
}