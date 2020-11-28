using System;
using Feats.CQRS.Events;

namespace Feats.Management.Metrics.Events
{
    public class FeatureUsedEvent : IEvent
    {
        public string Type => "feature.used";

        // todo
    }
}
