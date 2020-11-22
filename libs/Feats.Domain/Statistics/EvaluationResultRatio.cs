using System;

namespace Feats.Domain.Statistics 
{
    public record EvaluationRatio(
        string FeatureName,
        string Path, 
        DateTime LastUpdatedOn,
        decimal RatioOn,
        decimal RatioOff
    );
}