using System;
using System.Collections.Generic;

namespace Feats.Domain 
{
    public interface IFeature
    {
        string Name { get; }

        string Path { get; }

        DateTimeOffset CreatedOn { get; }

        string CreatedBy { get; }

        FeatureState State { get; }

        IDictionary<string, string> Strategies { get; }
    }
    
    // a record would be nice...
    public sealed class Feature : IFeature
    {
        public Feature()
        {
            this.Strategies = new Dictionary<string, string>();
        }

        public string Name {get; set;}

        public string Path {get; set;}

        public DateTimeOffset CreatedOn {get; set;}

        public DateTimeOffset UpdatedOn {get; set;}

        public string CreatedBy {get; set;}

        public FeatureState State {get; set;}

        public IDictionary<string, string> Strategies {get; set;}
    }
}