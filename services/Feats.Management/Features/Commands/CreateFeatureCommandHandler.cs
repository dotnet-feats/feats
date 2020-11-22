using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Json;

using EventStore.Client;
using Feats.CQRS.Commands;
using Feats.Domain.Validations;
using Microsoft.Extensions.Logging;
using Feats.Management.Features.Events;
using Feats.Management.EventStoreSetups;
using Feats.CQRS.Streams;
using Microsoft.Extensions.Internal;
using Feats.CQRS.Events;
using System.IO;
using Feats.Management.Features.Exceptions;
using System.Text;

namespace Feats.Management.Features.Commands
{
    public sealed class CreateFeatureCommand : ICommand
    {
        public string Name { get; set; }

        public string Path { get; set; }
        
        public string CreatedBy { get; set; }
        
        public IEnumerable<string> StrategyNames { get; set; }
    }

    public class CreateFeatureCommandHandler : IHandleCommand<CreateFeatureCommand>
    {
        private readonly ILogger _logger;

        private readonly IEventStoreClient _client;

        private readonly ISystemClock _clock;

        private readonly IStream _featureStream = new FeatureStream();

        private readonly IStream _pathStream = new PathStream();

        public CreateFeatureCommandHandler(
            ILogger<CreateFeatureCommandHandler> logger,
            IEventStoreClient client,
            ISystemClock clock)
        {
            this._logger = logger;
            this._client = client;
            this._clock = clock;
        }

        public async Task Handle(CreateFeatureCommand command)
        {
            command.Validate();
            this._logger.LogDebug($"CreateFeatureCommand validated, now checking for duplicates...");
            await this.ValidateNoDuplicateNames(command);
            
            this._logger.LogDebug($"CreateFeatureCommand no duplicates found, sending creation events");
            var featureCreatedEvent = new FeatureCreatedEvent
            {
                Name = command.Name,
                CreatedBy = command.CreatedBy,
                CreatedOn = this._clock.UtcNow,
                Path = command.Path,
                StrategyNames = command.StrategyNames,
            };
            
            var pathCreatedEvent = new PathCreatedEvent
            {
                CreatedBy = command.CreatedBy,
                CreatedOn = this._clock.UtcNow,
                Path = command.Path,
                FeatureAdded = command.Name,
            };

            var contentBytes = JsonSerializer.SerializeToUtf8Bytes(featureCreatedEvent);
            var featureCreatedEventPayload = new EventData(
                eventId: Uuid.NewUuid(),
                type : featureCreatedEvent.Type,
                data: contentBytes
            );

            var pathCcontentBytes = JsonSerializer.SerializeToUtf8Bytes(pathCreatedEvent);
            var pathCreatedEventPayload = new EventData(
                eventId: Uuid.NewUuid(),
                type : pathCreatedEvent.Type,
                data: pathCcontentBytes
            );

            await this._client.AppendToStreamAsync(
                stream: this._featureStream,
                expectedState: StreamState.Any,
                eventData: new List<EventData> {
                    featureCreatedEventPayload,
                }
            );
            this._logger.LogDebug($"FeatureCreatedEvent {featureCreatedEventPayload.EventId} sent!");

            await this._client.AppendToStreamAsync(
                stream: this._pathStream,
                expectedState: StreamState.Any,
                eventData: new List<EventData> {
                    pathCreatedEventPayload,
                }
            );
            this._logger.LogDebug($"PathCreatedEvent {pathCreatedEventPayload.EventId} sent!");
        }

        private async Task ValidateNoDuplicateNames(CreateFeatureCommand command)
        {
            var pathAndName = Path.Combine(command.Path, command.Name);

            var events = this._client.ReadStreamAsync(
                this._featureStream,
                Direction.Forwards,
                StreamPosition.Start);
                
            var state = await events.ReadState;
            if (state == ReadState.Ok)
            {
                // todo, metrics on how long this takes me, pretty confident though
                await foreach (var @event in events) {
                    if (@event.Event.EventType == EventTypes.FeatureCreated) 
                    {
                        var json = Encoding.UTF8.GetString(@event.Event.Data.ToArray());
                        var deserialized = JsonSerializer
                            .Deserialize<FeatureCreatedEvent>(json);
                        
                        var existingPathAndName = Path.Combine(deserialized.Path, deserialized.Name);

                        if (string.Equals(
                            pathAndName, 
                            existingPathAndName, 
                            StringComparison.InvariantCultureIgnoreCase))
                        {
                            throw new FeatureAlreadyExistsException();
                        }
                    }
                }
            }
        }
    }

    public static class CreateFeatureCommandExtensions 
    {
        public static void Validate(this CreateFeatureCommand command)
        {
            command.Required(nameof(command));
            command.Name.Required(nameof(command.Name));
            command.CreatedBy.Required(nameof(command.CreatedBy));
        }
    }
}
