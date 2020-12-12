using System;
using System.Diagnostics.CodeAnalysis;
using Feats.CQRS.Events;

namespace Feats.Domain.Events
{
    [ExcludeFromCodeCoverage]
    public sealed class StrategyUnAssignedEvent : IEvent
    {
        public string Type => EventTypes.StrategyUnassigned;

        public string Name { get; set; }
        
        public string Path { get; set; }

        public string UnassignedBy  { get; set; }
        
        public DateTimeOffset UnassignedOn  { get; set; }

        public string StrategyName { get; set; }
    }
}