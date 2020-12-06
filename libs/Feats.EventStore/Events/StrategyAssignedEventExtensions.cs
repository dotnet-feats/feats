using System;
using System.Text.Json;
using EventStore.Client;
using Feats.Domain.Events;

namespace Feats.EventStore.Events
{
    public static class StrategyAssignedEventExtensions
    {
        public static EventData ToEventData(this StrategyAssignedEvent assignedEvent, JsonSerializerOptions settings = null)
        {
            var contentBytes = JsonSerializer.SerializeToUtf8Bytes(assignedEvent, settings);
            return new EventData(
                eventId: Uuid.NewUuid(),
                type : assignedEvent.Type,
                data: contentBytes
            );
        }
    }
}
