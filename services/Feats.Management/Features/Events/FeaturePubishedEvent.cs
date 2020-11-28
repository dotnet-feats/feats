using System;
using System.Collections.Generic;
using System.Text.Json;
using EventStore.Client;
using Feats.CQRS.Events;

namespace Feats.Management.Features.Events
{
    public class FeaturePublishedEvent : IEvent
    {
        public string Type => EventTypes.FeaturePublished;

        public string Name { get; set; }

        public string Path  { get; set; }

        public string PublishedBy { get; set; }

        public DateTimeOffset PublishedOn { get; set; }
    }
    

    public static class FeaturePublishedEventExtensions
    {
        public static EventData ToEventData(this FeaturePublishedEvent featurePublishedEvent, JsonSerializerOptions settings = null)
        {
            var contentBytes = JsonSerializer.SerializeToUtf8Bytes(featurePublishedEvent, settings);
            return new EventData(
                eventId: Uuid.NewUuid(),
                type : featurePublishedEvent.Type,
                data: contentBytes
            );
        }
    }
}
