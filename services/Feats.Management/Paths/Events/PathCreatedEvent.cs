using System;
using Feats.CQRS.Events;

namespace Feats.Management.Features.Events
{
    // todo switch to record
    public class PathCreatedEvent
    {
        public string Type => EventTypes.PathCreated;

        public string FeatureAdded  { get; set; }

        public string Path  { get; set; }

        public string CreatedBy { get; set; }

        public DateTimeOffset CreatedOn  { get; set; }
    }
}
