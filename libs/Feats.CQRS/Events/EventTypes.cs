using System;
using System.Diagnostics.CodeAnalysis;

namespace Feats.CQRS.Events
{
    [ExcludeFromCodeCoverage]
    public static class EventTypes
    {
        public const string FeatureCreated = "event.features.created";

        public const string FeatureUpdated = "event.features.updated";
        
        public const string FeaturePublished = "event.features.published";

        public const string StrategyAssigned = "event.features.strategy.assigned";

        public const string StrategyUnassigned = "event.features.strategy.unassigned";

        public const string PathCreated = "event.paths.created";
    }
}
