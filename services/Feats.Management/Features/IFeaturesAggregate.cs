using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventStore.Client;
using Feats.CQRS;
using Feats.CQRS.Events;
using Feats.CQRS.Streams;
using Feats.Domain;
using Feats.Domain.Events;
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

                case FeaturePublishedEvent publishedEvent:
                    return publishedEvent.ToEventData();

                case StrategyAssignedEvent assignedEvent:
                    return assignedEvent.ToEventData();

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

                case FeaturePublishedEvent publishedEvent:
                    this.Apply(publishedEvent);
                    break;

                case StrategyAssignedEvent assignedEvent:
                    this.Apply(assignedEvent);
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
                UpdatedOn = e.CreatedOn,
                Path = e.Path,
                State = FeatureState.Draft,
                Strategies = new Dictionary<string, string>(),
            });
        }

        private void Apply(FeaturePublishedEvent e)
        {
            // the fact that you "can" publish an unkown feature is wanted
            // basically its a NOOP, so I dont care :P Though luck if you don't agree
            var pathAndName = System.IO.Path.Combine(e.Path, e.Name);
            var features = this.Features
                .Select(f => 
                {
                    if(System.IO.Path.Combine(f.Path, f.Name).Equals(pathAndName))
                    {
                        return new Feature
                        {
                            Name = e.Name,
                            CreatedBy = f.CreatedBy,
                            CreatedOn = f.CreatedOn,
                            UpdatedOn = e.PublishedOn,
                            Path = e.Path,
                            State = FeatureState.Published,
                            Strategies = f.Strategies,
                        };
                    }

                    return f;
                });

            this.Features = features;
        }

        private void Apply(StrategyAssignedEvent e)
        {
            // the fact that you "can" publish an unkown feature is wanted
            // basically its a NOOP, so I dont care :P Though luck if you don't agree
            var pathAndName = System.IO.Path.Combine(e.Path, e.Name);
            var features = this.Features
                .Select(f => 
                {
                    if(System.IO.Path.Combine(f.Path, f.Name).Equals(pathAndName))
                    {
                        if (!f.Strategies.ContainsKey(e.StrategyName))
                        {
                            f.Strategies.Add(e.StrategyName, e.Settings);
                        }
                        else 
                        {
                            f.Strategies[e.StrategyName] = e.Settings;
                        }

                        return new Feature
                        {
                            Name = e.Name,
                            CreatedBy = f.CreatedBy,
                            CreatedOn = f.CreatedOn,
                            UpdatedOn = e.AssignedOn,
                            Path = e.Path,
                            State = FeatureState.Published,
                            Strategies = f.Strategies,
                        };
                    }

                    return f;
                });

            this.Features = features;
        }
    }
}