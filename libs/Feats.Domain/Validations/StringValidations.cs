using System;
using System.Diagnostics.CodeAnalysis;
using Feats.Domain.Exceptions;

namespace Feats.Domain.Validations
{
    [ExcludeFromCodeCoverage]
    public static class StringValidationsExtensions
    {
        public static void Required(this string n, string name)
        {
            if (String.IsNullOrEmpty(n))
            {
                throw new ArgumentValidationException(name);
            }
        }
    }
}
