using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Feats.CQRS;
using Feats.CQRS.Events;
using Feats.CQRS.Streams;
using Feats.Domain;
using Feats.Domain.Events;
using Feats.EventStore;
using Microsoft.Extensions.Logging;

namespace Feats.Evaluations.Features
{
    public interface IFeaturesAggregate : IReadonlyAggregate
    {
        IEnumerable<IFeature> Features { get; }
    }

    public class FeaturesAggregate : IReadonlyAggregate, IFeaturesAggregate
    {
        private readonly ILogger _logger;

        private readonly IReadStreamedEvents<FeatureStream> _reader;

        private readonly IEventStoreClient _client;

        private readonly FeatureStream _featureStream;

        public FeaturesAggregate(
            ILogger<FeaturesAggregate> logger, 
            IReadStreamedEvents<FeatureStream> reader
        )
        {
            this._logger = logger;
            this._reader = reader;
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

                default:
                    break;
            }
        }

        private void Apply(FeatureCreatedEvent e)
        {
            this.Features = this.Features.Append(new Feature{
                Name = e.Name,
                CreatedBy = e.CreatedBy,
                CreatedOn = e.CreatedOn,
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