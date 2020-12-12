using System.Text.Json;
using EventStore.Client;
using Feats.Domain.Events;

namespace Feats.EventStore.Events
{
    public static class PathCreatedEventExtensions
    {
        public static EventData ToEventData(this PathCreatedEvent pathCreatedEvent, JsonSerializerOptions settings = null!)
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