using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using EventStore.Client;
using Feats.CQRS.Events;
using Feats.CQRS.Streams;
using Feats.Domain.Events;
using Microsoft.Extensions.Logging;

namespace Feats.EventStore
{
    internal sealed class PathStreamEventsReader : IReadStreamedEvents<PathStream>
    {
        private readonly IEventStoreClient _client;

        private readonly IStream _pathStream;

        private readonly ILogger<PathStreamEventsReader> _logger;

        public PathStreamEventsReader(
            ILogger<PathStreamEventsReader> logger,
            IEventStoreClient eventStoreClient)
        {
            this._logger = logger;
            this._client = eventStoreClient;
            this._pathStream = new PathStream();
        }
        
        public async IAsyncEnumerable<IEvent> Read()
        {
            var events = this._client.ReadStreamAsync(
                this._pathStream,
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

        private IEvent? FromResolvedEvent(ResolvedEvent @event)
        {
            var json = Encoding.UTF8.GetString(@event.Event.Data.ToArray());

            switch(@event.Event.EventType)
            {
                case EventTypes.PathCreated:
                    return JsonSerializer.Deserialize<PathCreatedEvent>(json);
                default:
                    this._logger.LogWarning($"No event registered for type: {@event.Event.EventType}");
                    
                    return null;
            }        
        }
    }
}
