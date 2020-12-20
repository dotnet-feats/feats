using System;
using System.Globalization;

namespace Feats.Evaluation.Client
{
    internal static class DoubleExtensions
    {
        internal static string ToInvariantString(this double value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }
    }
}