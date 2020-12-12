using System;
using System.Diagnostics.CodeAnalysis;

namespace Feats.Domain
{
    public interface IPath
    {
        string Name { get; }

        int TotalFeatures { get; }
    }
    

    [ExcludeFromCodeCoverage]
    public sealed class Path : IPath
    {
        public Path()
        {
            this.TotalFeatures =  0;
        }
        
        public int TotalFeatures { get; set; }

        public string Name {get; set;}
    }
}