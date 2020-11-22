using System;

namespace Feats.Domain.Validations
{
    public static class ObjectValidationsExtensions
    {
        public static void Required(this object n, string name)
        {
            if (n == null)
            {
                throw new ArgumentNullException(name);
            }
        }
    }
}
