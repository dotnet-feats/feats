using System;
using System.Collections.Generic;
using System.Text.Json;
using EventStore.Client;
using Feats.CQRS.Events;
using Feats.Domain.Events;

namespace Feats.Management.Features.Events
{
    public static class StrategyAssignedEventExtensions
    {
        public static EventData ToEventData(this StrategyAssignedEvent assignedEvent, JsonSerializerOptions settings = null)
        {
            var contentBytes = JsonSerializer.SerializeToUtf8Bytes(assignedEvent, settings);
            return new EventData(
                eventId: Uuid.NewUuid(),
                type : assignedEvent.Type,
                data: contentBytes
            );
        }
    }
}
