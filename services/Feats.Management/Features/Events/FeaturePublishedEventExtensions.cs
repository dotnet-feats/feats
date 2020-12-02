using System;
using System.Collections.Generic;
using System.Text.Json;
using EventStore.Client;
using Feats.CQRS.Events;
using Feats.Domain.Events;

namespace Feats.Management.Features.Events
{
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
