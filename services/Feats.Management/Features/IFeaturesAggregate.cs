using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EventStore.Client;
using Feats.CQRS;
using Feats.CQRS.Events;
using Feats.CQRS.Streams;
using Feats.Domain;
using Feats.EventStore;
using Feats.Management.Features.Events;
using Feats.Management.Features.Exceptions;
using Microsoft.Extensions.Logging;

namespace Feats.Management.Features
{
    public interface IFeaturesAggregate : IAggregate
    {
        IEnumerable<IFeature> Features { get; }
    }

    public class FeaturesAggregate : IAggregate, IFeaturesAggregate
    {
        private readonly ILogger _logger;

        private readonly IReadStreamedEvents<FeatureStream> _reader;

        private readonly IEventStoreClient _client;

        private readonly FeatureStream _featureStream;

        public FeaturesAggregate(
            ILogger<FeaturesAggregate> logger, 
            IReadStreamedEvents<FeatureStream> reader,
            IEventStoreClient client
        )
        {
            this._logger = logger;
            this._reader = reader;
            this._client = client;
            this._featureStream = new FeatureStream();
            this.Features = Enumerable.Empty<IFeature>();
        }

        public IEnumerable<IFeature> Features { get; private set;}

        public async Task Load()
        {
            var events = this._reader.Read();
            await foreach (var @event in events) 
            {
                this.Apply(@event);
            }
        }
        
        public async Task Publish(IEvent e)
        {
            this.Apply(e);
            var eventData = this.ToEventData(e);

            await this._client.AppendToStreamAsync(
                stream: this._featureStream,
                expectedState: StreamState.Any,
                eventData: new List<EventData> {
                    eventData,
                }
            );

            this._logger.LogDebug($"event {e.GetType().AssemblyQualifiedName} sent!");
        }

        private EventData ToEventData(IEvent e)
        {
            switch(e)
            {
                case FeatureCreatedEvent createdEvent:
                    return createdEvent.ToEventData();

                default:
                    return null;
            }
        }

        private void Apply(IEvent e)
        {
            switch(e)
            {
                case FeatureCreatedEvent createdEvent:
                    this.Apply(createdEvent);
                    break;

                default:
                    break;
            }
        }

        private void Apply(FeatureCreatedEvent e)
        {
            var pathAndName = System.IO.Path.Combine(e.Path, e.Name);
            var allExistingPathsAndNames = this.Features.Select(f => System.IO.Path.Combine(f.Path, f.Name));
            
            if (allExistingPathsAndNames.Contains(pathAndName))
            {
                throw new FeatureAlreadyExistsException();
            }

            this.Features = this.Features.Append(new Feature{
                Name = e.Name,
                CreatedBy = e.CreatedBy,
                CreatedOn = e.CreatedOn,
                Path = e.Path,
                State = FeatureState.Draft,
                StrategyNames = e.StrategyNames,
            });
        }
    }
}