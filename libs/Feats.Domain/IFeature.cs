using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Feats.Domain 
{
    public interface IFeature
    {
        string Name { get; }

        string Path { get; }

        DateTimeOffset CreatedOn { get; }

        DateTimeOffset UpdatedOn { get; }

        string CreatedBy { get; }

        FeatureState State { get; }

        IEnumerable<IFeatureStrategy> Strategies { get; }
    }

    public interface IFeatureStrategy
    {
        string Name { get; }
        
        string Value { get; }
    }

    public sealed class FeatureStrategy : IFeatureStrategy
    {
        public string Name { get; set; }
        
        public string Value { get; set; }
    }
    
    [ExcludeFromCodeCoverage]
    // a record would be nice...
    public sealed class Feature : IFeature
    {
        public Feature()
        {
            this.Strategies = Enumerable.Empty<IFeatureStrategy>();
            this.State = FeatureState.Draft;
        }

        public string Name {get; set;}

        public string Path {get; set;}

        public DateTimeOffset CreatedOn {get; set;}

        public DateTimeOffset UpdatedOn {get; set;}

        public string CreatedBy {get; set;}

        public FeatureState State {get; set;}

        public IEnumerable<IFeatureStrategy> Strategies {get; set;}
    }
}