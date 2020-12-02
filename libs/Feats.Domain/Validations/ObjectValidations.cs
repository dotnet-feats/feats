using System;
using System.Diagnostics.CodeAnalysis;

namespace Feats.Domain.Validations
{
    [ExcludeFromCodeCoverage]
    public static class ObjectValidationsExtensions
    {
        public static void Required(this object n, string name)
        {
            if (n == null)
            {
                throw new ArgumentValidationException(name);
            }
        }
    }
}
