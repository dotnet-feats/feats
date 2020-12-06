using System;
using System.Diagnostics.CodeAnalysis;
using Feats.CQRS.Events;

namespace Feats.Domain.Events
{
    [ExcludeFromCodeCoverage]
    public sealed class FeatureArchivedEvent : IEvent
    {
        public string Type => EventTypes.FeatureArchived;

        public string Name { get; set; }

        public string Path  { get; set; }

        public string ArchivedBy { get; set; }

        public DateTimeOffset ArchivedOn { get; set; }
    }
}