using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Feats.Domain
{
    public static class PathHelper
    {
        public static string CombineNameAndPath(string path, string name)
        {
            var first = PathDelimiters.First(path);
            var defaultPath = string.IsNullOrEmpty(path) ? string.Empty : path.Trim(first.ToCharArray());
            var defaultName = string.IsNullOrEmpty(name) ? string.Empty : name.Trim(first.ToCharArray());

            return $"{defaultPath}{first}{defaultName}".Trim(first.ToCharArray());
        }

        public static IEnumerable<string> TranformToPathLevels(string path)
        {
            var first = PathDelimiters.First(path);
            
            var trimmed = path.Trim(first.ToCharArray());
            var matches = Regex
                .Matches(trimmed, $"[{Regex.Escape(first)}]");

            return matches
                .Select(match => trimmed.Substring(0, match.Index))
                .Append(trimmed)
                .Distinct()
                .ToList();
        }
    }
}