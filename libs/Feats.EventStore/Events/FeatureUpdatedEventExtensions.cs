using System;
using System.Text.Json;
using EventStore.Client;
using Feats.Domain.Events;

namespace Feats.EventStore.Events
{
    public static class FeatureUpdatedEventExtensions
    {
        public static EventData ToEventData(this FeatureUpdatedEvent featureUpdatedEvent, JsonSerializerOptions settings = null)
        {
            var contentBytes = JsonSerializer.SerializeToUtf8Bytes(featureUpdatedEvent, settings);
            return new EventData(
                eventId: Uuid.NewUuid(),
                type : featureUpdatedEvent.Type,
                data: contentBytes
            );
        }
    }
}
