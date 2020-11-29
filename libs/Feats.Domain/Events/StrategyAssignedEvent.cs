using System;
using System.Diagnostics.CodeAnalysis;
using Feats.CQRS.Events;

namespace Feats.Domain.Events
{
    [ExcludeFromCodeCoverage]
    public sealed class StrategyAssignedEvent : IEvent
    {
        public string Type => EventTypes.StrategyAssigned;

        public string Name { get; set; }
        
        public string Path { get; set; }

        public string AssignedBy  { get; set; }
        
        public DateTimeOffset AssignedOn  { get; set; }

        public string StrategyName { get; set; }

        public string Settings { get; set; }
    }
}