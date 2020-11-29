using System;
using System.Collections.Generic;
using System.Text.Json;
using EventStore.Client;
using Feats.CQRS.Events;
using Feats.Domain.Events;

namespace Feats.Management.Features.Events
{
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
