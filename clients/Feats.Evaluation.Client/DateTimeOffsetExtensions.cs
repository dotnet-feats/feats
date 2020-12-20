using System;
using System.Globalization;

namespace Feats.Evaluation.Client
{
    internal static class DateTimeOffsetExtensions
    {
        internal static string ToIsoStopBuggingMeFormat(this DateTimeOffset date)
        {
            return date.ToString("O", CultureInfo.InvariantCulture);
        }
    }
}