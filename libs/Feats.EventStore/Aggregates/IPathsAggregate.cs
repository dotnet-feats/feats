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
using Feats.EventStore.Events;
using Microsoft.Extensions.Logging;

namespace Feats.EventStore.Aggregates
{
    public interface IPathsAggregate : IAggregate
    {
        IEnumerable<IPath> Paths { get; }
    }

    public class PathsAggregate : IAggregate, IPathsAggregate
    {
        private readonly ILogger _logger;

        private readonly IReadStreamedEvents<PathStream> _reader;

        private readonly IEventStoreClient _client;

        private readonly PathStream _pathStream;

        public PathsAggregate(
            ILogger<PathsAggregate> logger, 
            IReadStreamedEvents<PathStream> reader,
            IEventStoreClient client
        )
        {
            this._logger = logger;
            this._reader = reader;
            this._client = client;
            this._pathStream = new PathStream();
            this.Paths = Enumerable.Empty<IPath>();
        }

        public IEnumerable<IPath> Paths { get; private set;}

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
                stream: this._pathStream,
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
                case PathCreatedEvent createdEvent:
                    return createdEvent.ToEventData();

                case PathRemovedEvent removedEvent:
                    return removedEvent.ToEventData();

                default:
                    return null;
            }
        }

        private void Apply(IEvent e)
        {
            switch(e)
            {
                case PathCreatedEvent createdEvent:
                    this.Apply(createdEvent);
                    break;

                case PathRemovedEvent removedEvent:
                    this.Apply(removedEvent);
                    break;

                default:
                    break;
            }
        }

        private void Apply(PathRemovedEvent e)
        {
            if(this.Paths.Any(_ => _.Name.Equals(e.Path, StringComparison.InvariantCultureIgnoreCase))){
                var sections = PathHelper.TranformToPathLevels(e.Path);
                var existingPaths = this.Paths
                    .Where(_ => sections.Contains(_.Name))
                    .Select(_ => _.Name)
                    .ToList();
                if(existingPaths.Any())
                {
                    this.Paths = this.Paths
                        .Select(p => 
                        {
                            if (sections.Contains(p.Name))
                            {
                                return new Domain.Path
                                {
                                    Name = p.Name,
                                    TotalFeatures = p.TotalFeatures - 1,
                                };
                            }

                            return p;
                        })
                        .Where(_ => _.TotalFeatures > 0)
                        .ToList();
                }
            }
        }

        private void Apply(PathCreatedEvent e)
        {
            var sections = PathHelper.TranformToPathLevels(e.Path);

            var existingPaths = this.Paths
                .Where(_ => sections.Contains(_.Name))
                .Select(_ => _.Name)
                .ToList();
            var missingSections = sections
                .Except(existingPaths)
                .ToList();
            if(existingPaths.Any())
            {
                this.Paths = this.Paths
                    .Select(p => 
                    {
                        if (sections.Contains(p.Name))
                        {
                            return new Domain.Path
                            {
                                Name = p.Name,
                                TotalFeatures = p.TotalFeatures + 1,
                            };
                        }

                        return p;
                    })
                    .ToList();

                if(missingSections.Any())
                {
                    this.Paths = this.Paths.Concat(
                        missingSections.Select(_ =>
                            new Domain.Path
                            {
                                    Name = _,
                                    TotalFeatures = 1,
                            }))
                            .ToList();
                }
            }
            else if(!existingPaths.Any()) {
                this.Paths = this.Paths.Concat(
                    sections.Select(_ =>
                    new Domain.Path
                    {
                            Name = _,
                            TotalFeatures = 1,
                    }))
                    .ToList();
            }
        }
    }
}