using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Text;
using System.Text.Json;

using EventStore.Client;
using Feats.CQRS.Commands;
using Feats.CQRS.Events;
using Feats.Domain.Validations;
using Feats.Management.EventStoreSetups;
using Feats.Management.Features.Events;
using Feats.Management.Features.Exceptions;
using Feats.CQRS.Streams;

using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Logging;
using Feats.Management.Paths;

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

        private readonly IFeaturesAggregate _featuresAggregate;

        private readonly IPathsAggregate _pathsAggregate;

        private readonly IEventStoreClient _client;

        private readonly ISystemClock _clock;

        public CreateFeatureCommandHandler(
            ILogger<CreateFeatureCommandHandler> logger,
            IFeaturesAggregate featuresAggregate,
            IPathsAggregate pathAggregate,
            ISystemClock clock)
        {
            this._logger = logger;
            this._featuresAggregate = featuresAggregate;
            this._pathsAggregate = pathAggregate;
            this._clock = clock;
        }

        public async Task Handle(CreateFeatureCommand command)
        {
            command.Validate();
            await this._featuresAggregate.Load();
                       
            var featureCreatedEvent = command
                .ExtractFeatureCreatedEvent(this._clock);

            await this._featuresAggregate.Publish(featureCreatedEvent);

            var pathCreatedEvent = command
                .ExtractPathCreatedEvent(this._clock);

            await this._pathsAggregate.Publish(pathCreatedEvent);
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

        public static FeatureCreatedEvent ExtractFeatureCreatedEvent(this CreateFeatureCommand command, ISystemClock clock)
        {
            return new FeatureCreatedEvent
            {
                Name = command.Name,
                CreatedBy = command.CreatedBy,
                CreatedOn = clock.UtcNow,
                Path = command.Path,
                StrategyNames = command.StrategyNames,
            };
        }
        

        public static PathCreatedEvent ExtractPathCreatedEvent(this CreateFeatureCommand command, ISystemClock clock)
        {
            return new PathCreatedEvent
            {
                CreatedBy = command.CreatedBy,
                CreatedOn = clock.UtcNow,
                Path = command.Path,
                FeatureAdded = command.Name,
            };
        }
    }
}
