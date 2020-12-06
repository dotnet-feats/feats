using System;
using System.Text.Json;
using Feats.CQRS.Events;

namespace Feats.Domain.Events
{
    // todo switch to record
    public class PathCreatedEvent : IEvent
    {
        public string Type => EventTypes.PathCreated;

        public string FeatureAdded  { get; set; }

        public string Path  { get; set; }

        public string CreatedBy { get; set; }

        public DateTimeOffset CreatedOn  { get; set; }
    }
}
