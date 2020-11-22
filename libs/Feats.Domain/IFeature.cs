using System;
using System.Collections.Generic;

namespace Feats.Domain 
{
    public interface IFeature
    {
        string Name { get; }

        string Path { get; }

        DateTime CreatedOn { get; }

        string CreatedBy { get; }

        FeatureState State { get; }

        IEnumerable<string> StrategyNames { get; }
    }
}