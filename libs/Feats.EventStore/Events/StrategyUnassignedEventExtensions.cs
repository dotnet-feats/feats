using System;
using System.Text.Json;
using EventStore.Client;
using Feats.Domain.Events;

namespace Feats.EventStore.Events
{
    public static class StrategyUnassignedEventExtensions
    {
        public static EventData ToEventData(this StrategyUnAssignedEvent unAssignedEvent, JsonSerializerOptions settings = null!)
        {
            var contentBytes = JsonSerializer.SerializeToUtf8Bytes(unAssignedEvent, settings);
            return new EventData(
                eventId: Uuid.NewUuid(),
                type : unAssignedEvent.Type,
                data: contentBytes
            );
        }
    }
}
