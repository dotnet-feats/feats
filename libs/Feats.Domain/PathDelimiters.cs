using System.Collections.Generic;
using System.Linq;

namespace Feats.Domain
{
    public static class PathDelimiters
    {
        public static readonly string[] Delimiters = new string[6]
        {
            ".",
            "_",
            "/",
            ",",
            "\\",
            " ",
        };

        public static string First(string path)
        {
            if(string.IsNullOrEmpty(path))
            {
                return Delimiters.First();
            }

            var firstDelimiter = Delimiters
                .Select(d => new KeyValuePair<string, int>(d, path.IndexOf(d)))
                .Where(_ => _.Value >= 0)
                .OrderBy(kv => kv.Value)
                .Select(_ => _.Key)
                .FirstOrDefault();
            
            return firstDelimiter ?? Delimiters.First();
        }
    }
}