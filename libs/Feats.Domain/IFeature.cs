using System;
using System.Collections.Generic;
using System.Linq;

namespace Feats.Domain 
{
    public interface IFeature
    {
        string Name { get; }

        string Path { get; }

        DateTimeOffset CreatedOn { get; }

        string CreatedBy { get; }

        FeatureState State { get; }

        IEnumerable<string> StrategyNames { get; }
    }
    
    // a record would be nice...
    public sealed class Feature : IFeature
    {
        public Feature()
        {
            this.StrategyNames = Enumerable.Empty<string>();
        }

        public string Name {get; set;}

        public string Path {get; set;}

        public DateTimeOffset CreatedOn {get; set;}

        public string CreatedBy {get; set;}

        public FeatureState State {get; set;}

        public IEnumerable<string> StrategyNames {get; set;}
    }
}