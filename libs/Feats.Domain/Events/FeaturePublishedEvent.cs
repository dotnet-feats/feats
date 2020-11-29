using System;
using Feats.CQRS.Events;

namespace Feats.Domain.Events
{
    public class FeaturePublishedEvent : IEvent
    {
        public string Type => EventTypes.FeaturePublished;

        public string Name { get; set; }

        public string Path  { get; set; }

        public string PublishedBy { get; set; }

        public DateTimeOffset PublishedOn { get; set; }
    }
}