using System;

namespace Feats.Domain.Statistics 
{
    public record EvaluationCount(
        string FeatureName,
        string Path, 
        DateTime LastUpdatedOn,
        long Count
    );
}