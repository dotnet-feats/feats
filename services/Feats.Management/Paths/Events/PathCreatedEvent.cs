using System;
using System.Text.Json;
using EventStore.Client;
using Feats.CQRS.Events;

namespace Feats.Management.Features.Events
{
    // todo switch to record
    public class PathCreatedEvent : IEvent
    {
        public string Type => EventTypes.PathCreated;

        public string FeatureAdded  { get; set; }

        public string Path  { get; set; }

        public string CreatedBy { get; set; }

        public DateTimeOffset CreatedOn  { get; set; }
    }

    public static class PathCreatedEventExtensions
    {
        public static EventData ToEventData(this PathCreatedEvent pathCreatedEvent, JsonSerializerOptions settings = null)
        {
            var contentBytes = JsonSerializer.SerializeToUtf8Bytes(pathCreatedEvent, settings);
            return new EventData(
                eventId: Uuid.NewUuid(),
                type : pathCreatedEvent.Type,
                data: contentBytes
            );
        }
    }
}
