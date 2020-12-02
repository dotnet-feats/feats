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

    public static class PathHelper
    {
        public static string CombineNameAndPath(string path, string name)
        {
            var defaultPath = string.IsNullOrEmpty(path) ? string.Empty : path.Trim();
            var defaultName = string.IsNullOrEmpty(name) ? string.Empty : name.Trim();
            
            return System.IO.Path.Combine(defaultPath, defaultName);
        }
    }
}