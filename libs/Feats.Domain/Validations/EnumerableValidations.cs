using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Feats.Domain.Validations
{
    [ExcludeFromCodeCoverage]
    public static class EnumerableValidationsExtensions
    {
        public static void Required<T>(this IEnumerable<T> n, string name)
        {
            if (n == null || !n.Any())
            {
                throw new ArgumentValidationException(name);
            }
        }
    }
}
