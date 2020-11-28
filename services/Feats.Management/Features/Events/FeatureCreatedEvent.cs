using System;
using System.Collections.Generic;
using System.Text.Json;
using EventStore.Client;
using Feats.CQRS.Events;

namespace Feats.Management.Features.Events
{
    public class FeatureCreatedEvent : IEvent
    {
        public string Type => EventTypes.FeatureCreated;

        public string Name { get; set; }

        public string Path  { get; set; }

        public string CreatedBy  { get; set; }

        public DateTimeOffset CreatedOn { get; set; }

        public IEnumerable<string> StrategyNames { get; set; }
    }
    

    public static class FeatureCreatedEventExtensions
    {
        public static EventData ToEventData(this FeatureCreatedEvent featureCreatedEvent, JsonSerializerOptions settings = null)
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
