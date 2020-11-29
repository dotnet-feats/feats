
using System;
using System.Diagnostics.CodeAnalysis;
using Feats.CQRS.Events;

namespace Feats.Domain.Events
{
    [ExcludeFromCodeCoverage]
    public sealed class FeatureCreatedEvent : IEvent
    {
        public string Type => EventTypes.FeatureCreated;

        public string Name { get; set; }

        public string Path  { get; set; }

        public string CreatedBy  { get; set; }

        public DateTimeOffset CreatedOn { get; set; }
    }
}