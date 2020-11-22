using System;
using System.Collections.Generic;
using Feats.CQRS.Events;

namespace Feats.Management.Features.Events
{
    // todo switch to record
    public class FeatureCreatedEvent
    {
        public string Type => EventTypes.FeatureCreated;

        public string Name { get; set; }

        public string Path  { get; set; }

        public string CreatedBy  { get; set; }

        public DateTimeOffset CreatedOn { get; set; }

        public IEnumerable<string> StrategyNames { get; set; }
    }
}
