using System;
using System.Text.Json;
using EventStore.Client;
using Feats.Domain.Events;

namespace Feats.EventStore.Events
{
    public static class FeatureCreatedEventExtensions
    {
        public static EventData ToEventData(this FeatureCreatedEvent featureCreatedEvent, JsonSerializerOptions settings = null!)
        {
            var contentBytes = JsonSerializer.SerializeToUtf8Bytes(featureCreatedEvent, settings);
            return new EventData(
                eventId: Uuid.NewUuid(),
                type : featureCreatedEvent.Type,
                data: contentBytes
            );
        }
    }
}
