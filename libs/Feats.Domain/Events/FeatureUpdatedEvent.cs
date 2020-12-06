
using System;
using System.Diagnostics.CodeAnalysis;
using Feats.CQRS.Events;

namespace Feats.Domain.Events
{
    [ExcludeFromCodeCoverage]
    public sealed class FeatureUpdatedEvent : IEvent
    {
        public string Type => EventTypes.FeatureUpdated;

        public string Name { get; set; }

        public string Path { get; set; }

        public string UpdatedBy { get; set; }

        public DateTimeOffset UpdatedOn { get; set; }
    }
}