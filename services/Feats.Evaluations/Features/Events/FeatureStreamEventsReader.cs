using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using EventStore.Client;
using Feats.CQRS.Events;
using Feats.CQRS.Streams;
using Feats.Domain.Events;
using Feats.EventStore;
using Microsoft.Extensions.Logging;

namespace Feats.Evaluations.Features.Events
{
    public sealed class FeatureStreamEventsReader : IReadStreamedEvents<FeatureStream>
    {
        private readonly IEventStoreClient _client;

        private readonly IStream _featureStream;

        private readonly ILogger<FeatureStreamEventsReader> _logger;

        public FeatureStreamEventsReader(
            ILogger<FeatureStreamEventsReader> logger,
            IEventStoreClient eventStoreClient)
        {
            this._logger = logger;
            this._client = eventStoreClient;
            this._featureStream = new FeatureStream();
        }
        
        public async IAsyncEnumerable<IEvent> Read()
        {
            var events = this._client.ReadStreamAsync(
                this._featureStream,
                Direction.Forwards,
                StreamPosition.Start);
             
            if (events != null)
            {
                // todo, metrics on how long this takes me, pretty confident though
                await foreach (var @event in events) 
                {
                    var results = this.FromResolvedEvent(@event);
                    if (results != null)
                    {
                        yield return results;
                    }
                }
            }
        }

        private IEvent FromResolvedEvent(ResolvedEvent @event)
        {
            var json = Encoding.UTF8.GetString(@event.Event.Data.ToArray());

            switch(@event.Event.EventType)
            {
                case EventTypes.FeatureCreated:
                    return JsonSerializer.Deserialize<FeatureCreatedEvent>(json);
                    
                case EventTypes.FeaturePublished:
                    return JsonSerializer.Deserialize<FeaturePublishedEvent>(json);
                    
                case EventTypes.StrategyAssigned:
                    return JsonSerializer.Deserialize<StrategyAssignedEvent>(json);
                    
                default:
                    this._logger.LogWarning($"No event registered for type: {@event.Event.EventType}");
                    
                    return null;
            }        
        }
    }
}
