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
using Feats.EventStore.Exceptions;
using Microsoft.Extensions.Logging;

namespace Feats.EventStore.Aggregates
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

                case FeatureUpdatedEvent updatedEvent:
                    return updatedEvent.ToEventData();

                case FeaturePublishedEvent publishedEvent:
                    return publishedEvent.ToEventData();

                case FeatureArchivedEvent archivedEvent:
                    return archivedEvent.ToEventData();

                case StrategyAssignedEvent assignedEvent:
                    return assignedEvent.ToEventData();

                case StrategyUnassignedEvent unassignedEvent:
                    return unassignedEvent.ToEventData();

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

                case FeatureUpdatedEvent updatedEvent:
                    this.Apply(updatedEvent);
                    break;

                case FeaturePublishedEvent publishedEvent:
                    this.Apply(publishedEvent);
                    break;

                case FeatureArchivedEvent archivedEvent:
                    this.Apply(archivedEvent);
                    break;

                case StrategyAssignedEvent assignedEvent:
                    this.Apply(assignedEvent);
                    break;

                case StrategyUnassignedEvent unassignedEvent:
                    this.Apply(unassignedEvent);
                    break;

                default:
                    break;
            }
        }

        private void Apply(FeatureCreatedEvent e)
        {
            var pathAndName = PathHelper.CombineNameAndPath(e.Path, e.Name);
            var allExistingPathsAndNames = this.Features.Select(f => PathHelper.CombineNameAndPath(f.Path, f.Name));
            
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
            }).ToList();
        }

        private void Apply(FeatureUpdatedEvent e)
        {
            var pathAndName = PathHelper.CombineNameAndPath(e.Path, e.Name);

            var exists = this.Features.Any(_ => PathHelper.CombineNameAndPath(_.Path, _.Name).Equals(pathAndName));
            if (!exists)
            {
                throw new FeatureNotFoundException(e.Path, e.Name);
            }

            var features = this.Features
                .Select(f => 
                {
                    if(PathHelper.CombineNameAndPath(f.Path, f.Name).Equals(pathAndName))
                    {
                        if(f.State != FeatureState.Draft)
                        {
                            throw new FeatureWasPublishedBeforeException();
                        }

                        return new Feature
                        {
                            Name = e.NewName,
                            Path = e.NewPath,
                            UpdatedOn = e.UpdatedOn,
                            CreatedBy = f.CreatedBy,
                            CreatedOn = f.CreatedOn,
                            State = f.State,
                            Strategies = f.Strategies,
                        };
                    }

                    return f;
                });

            this.Features = features.ToList();
        }

        private void Apply(FeaturePublishedEvent e)
        {
            var pathAndName = PathHelper.CombineNameAndPath(e.Path, e.Name);
            
            var exists = this.Features.Any(_ => PathHelper.CombineNameAndPath(_.Path, _.Name).Equals(pathAndName));
            if (!exists)
            {
                throw new FeatureNotFoundException(e.Path, e.Name);
            }

            var features = this.Features
                .Select(f => 
                {
                    if(PathHelper.CombineNameAndPath(f.Path, f.Name).Equals(pathAndName))
                    {
                        return new Feature
                        {
                            Name = f.Name,
                            CreatedBy = f.CreatedBy,
                            CreatedOn = f.CreatedOn,
                            UpdatedOn = e.PublishedOn,
                            Path = f.Path,
                            State = FeatureState.Published,
                            Strategies = f.Strategies,
                        };
                    }

                    return f;
                });

            this.Features = features.ToList();
        }
        private void Apply(FeatureArchivedEvent e)
        {
            var pathAndName = PathHelper.CombineNameAndPath(e.Path, e.Name);
            
            var exists = this.Features.Any(_ => PathHelper.CombineNameAndPath(_.Path, _.Name).Equals(pathAndName));
            if (!exists)
            {
                throw new FeatureNotFoundException(e.Path, e.Name);
            }

            var features = this.Features
                .Select(f => 
                {
                    if(PathHelper.CombineNameAndPath(f.Path, f.Name).Equals(pathAndName))
                    {
                        return new Feature
                        {
                            Name = f.Name,
                            CreatedBy = f.CreatedBy,
                            CreatedOn = f.CreatedOn,
                            UpdatedOn = e.ArchivedOn,
                            Path = f.Path,
                            State = FeatureState.Archived,
                            Strategies = f.Strategies,
                        };
                    }

                    return f;
                });

            this.Features = features.ToList();
        }

        private void Apply(StrategyAssignedEvent e)
        {
            var pathAndName = PathHelper.CombineNameAndPath(e.Path, e.Name);
            
            var exists = this.Features.Any(_ => PathHelper.CombineNameAndPath(_.Path, _.Name).Equals(pathAndName));
            if (!exists)
            {
                throw new FeatureNotFoundException(e.Path, e.Name);
            }

            var features = this.Features
                .Select(f => 
                {
                    if(PathHelper.CombineNameAndPath(f.Path, f.Name).Equals(pathAndName))
                    {
                        if (f.State != FeatureState.Draft)
                        {
                            throw new FeatureWasPublishedBeforeException();
                        }

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
                            Name = f.Name,
                            CreatedBy = f.CreatedBy,
                            CreatedOn = f.CreatedOn,
                            UpdatedOn = e.AssignedOn,
                            Path = f.Path,
                            State = f.State,
                            Strategies = f.Strategies,
                        };
                    }

                    return f;
                });

            this.Features = features.ToList();
        }

        private void Apply(StrategyUnassignedEvent e)
        {
            var pathAndName = PathHelper.CombineNameAndPath(e.Path, e.Name);
            
            var exists = this.Features.Any(_ => PathHelper.CombineNameAndPath(_.Path, _.Name).Equals(pathAndName));
            if (!exists)
            {
                throw new FeatureNotFoundException(e.Path, e.Name);
            }
            
            var features = this.Features
                .Select(f => 
                {
                    if(PathHelper.CombineNameAndPath(f.Path, f.Name).Equals(pathAndName))
                    {
                        if (f.State != FeatureState.Draft)
                        {
                            throw new FeatureWasPublishedBeforeException();
                        }

                        if (f.Strategies.ContainsKey(e.StrategyName))
                        {
                            f.Strategies.Remove(e.StrategyName);
                        }

                        return new Feature
                        {
                            Name = f.Name,
                            CreatedBy = f.CreatedBy,
                            CreatedOn = f.CreatedOn,
                            UpdatedOn = e.UnassignedOn,
                            Path = f.Path,
                            State = f.State,
                            Strategies = f.Strategies,
                        };
                    }

                    return f;
                });

            this.Features = features.ToList();
        }
    }
}