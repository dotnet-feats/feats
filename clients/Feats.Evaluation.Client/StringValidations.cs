using System;
using System.Diagnostics.CodeAnalysis;

namespace Feats.Evaluation.Client
{
    [ExcludeFromCodeCoverage]
    internal static class StringValidationsExtensions
    {
        public static void Required(this string n, string name)
        {
            if (String.IsNullOrEmpty(n))
            {
                throw new ArgumentNullException(name);
            }
        }
    }
}
