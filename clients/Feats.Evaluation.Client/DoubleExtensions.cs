using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Feats.Evaluation.Client
{
    [ExcludeFromCodeCoverage]
    internal static class DoubleExtensions
    {
        internal static string ToInvariantString(this double value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }
    }
}