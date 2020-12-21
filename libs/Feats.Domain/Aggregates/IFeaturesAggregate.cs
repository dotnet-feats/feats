using System.Collections.Generic;
using Feats.CQRS;

namespace Feats.Domain.Aggregates
{
    public interface IFeaturesAggregate : IAggregate
    {
        IEnumerable<IFeature> Features { get; }
    }
}