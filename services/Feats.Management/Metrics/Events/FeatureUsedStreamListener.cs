using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using EventStore.Client;
using Feats.CQRS.Events;
using Feats.CQRS.Streams;
using Feats.EventStore;
using Microsoft.Extensions.Logging;

namespace Feats.Management.Features.Events
{
    public sealed class FeatureEventStreamListener : IListenToStream<MetricsStream>
    {
        private readonly ILogger<FeatureEventStreamListener> _logger;
        private readonly IEventStoreClient _eventStore;
        private readonly IEnumerable<IHandleEvent> _eventHandlers;
        private readonly IStream _stream;

        public FeatureEventStreamListener(
            ILogger<FeatureEventStreamListener> logger,
            IEventStoreClient eventStore, 
            IEnumerable<IHandleEvent> eventHandlers,
            string featureName)
        {
            this._logger = logger;
            this._eventStore = eventStore;
            this._eventHandlers = eventHandlers;
            this._stream = new MetricsStream(featureName);
        }

        public async Task Listen()
        {
            await this._eventStore.SubscribeToStreamAsync(
                this._stream,
                StreamPosition.Start,
                async (subscription, resolvedEvent, cancellationToken) => 
                {
                    if (!cancellationToken.IsCancellationRequested)
                    {
                        foreach (var handler in this._eventHandlers)
                        {
                            await handler.Handle(
                                resolvedEvent.Event.EventType,
                                Encoding.UTF8.GetString(resolvedEvent.Event.Data.ToArray()));
                        }
                    }
                }
            );
        }
    }
}
