using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Feats.Domain 
{
    public interface IPath
    {
        string Name { get; }

        IEnumerable<string> FeatureNames { get; }
    }
    

    [ExcludeFromCodeCoverage]
    public sealed class Path : IPath
    {
        public Path()
        {
            this.FeatureNames = Enumerable.Empty<string>();
        }
        
        public IEnumerable<string> FeatureNames { get; set; }

        public string Name {get; set;}
    }
}