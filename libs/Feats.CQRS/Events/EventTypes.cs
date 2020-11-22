using System;

namespace Feats.CQRS.Events
{
    public static class EventTypes
    {
        public const string FeatureCreated = "event.features.created";

        public const string PathCreated = "event.paths.created";
    }
}
