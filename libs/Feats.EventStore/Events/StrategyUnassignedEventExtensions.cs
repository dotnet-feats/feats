using System;
using System.Text.Json;
using EventStore.Client;
using Feats.Domain.Events;

namespace Feats.EventStore.Events
{
    public static class StrategyUnassignedEventExtensions
    {
        public static EventData ToEventData(this StrategyUnassignedEvent UnassignedEvent, JsonSerializerOptions settings = null)
        {
            var contentBytes = JsonSerializer.SerializeToUtf8Bytes(UnassignedEvent, settings);
            return new EventData(
                eventId: Uuid.NewUuid(),
                type : UnassignedEvent.Type,
                data: contentBytes
            );
        }
    }
}
