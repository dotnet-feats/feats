using System;
using System.Diagnostics.CodeAnalysis;
using Feats.Domain.Exceptions;

namespace Feats.Domain.Validations
{
    [ExcludeFromCodeCoverage]
    public static class DateTimeValidationsExtensions
    {
        public static void Required(this DateTimeOffset n, string name)
        {
            if (n == null || n == DateTimeOffset.MinValue)
            {
                throw new ArgumentValidationException(name);
            }
        }
    }
}
