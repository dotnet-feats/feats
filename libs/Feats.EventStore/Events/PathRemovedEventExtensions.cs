using System.Text.Json;
using EventStore.Client;
using Feats.Domain.Events;

namespace Feats.EventStore.Events
{
    public static class PathRemovedEventExtensions
    {
        public static EventData ToEventData(this PathRemovedEvent pathRemovedEvent, JsonSerializerOptions settings = null)
        {
            var contentBytes = JsonSerializer.SerializeToUtf8Bytes(pathRemovedEvent, settings);
            return new EventData(
                eventId: Uuid.NewUuid(),
                type : pathRemovedEvent.Type,
                data: contentBytes
            );
        }
    }
}