using System;

namespace Feats.Domain.Validations
{
    public static class StringValidationsExtensions
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
