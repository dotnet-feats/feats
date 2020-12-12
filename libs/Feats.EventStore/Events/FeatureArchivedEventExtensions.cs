using System;
using System.Text.Json;
using EventStore.Client;
using Feats.Domain.Events;

namespace Feats.EventStore.Events
{
    public static class FeatureArchivedEventExtensions
    {
        public static EventData ToEventData(this FeatureArchivedEvent featureArchivedEvent, JsonSerializerOptions settings = null!)
        {
            var contentBytes = JsonSerializer.SerializeToUtf8Bytes(featureArchivedEvent, settings);
            return new EventData(
                eventId: Uuid.NewUuid(),
                type : featureArchivedEvent.Type,
                data: contentBytes
            );
        }
    }
}
