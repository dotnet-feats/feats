using System;
using Feats.CQRS.Events;

namespace Feats.Domain.Events
{
    // todo switch to record
    public class PathRemovedEvent : IEvent
    {
        public string Type => EventTypes.PathRemoved;

        public string FeatureRemoved  { get; set; }

        public string Path  { get; set; }

        public string RemovedBy { get; set; }

        public DateTimeOffset RemovedOn  { get; set; }
    }
}
