using System;
using System.Threading.Tasks;

using Feats.CQRS.Commands;
using Feats.Domain.Validations;

using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Logging;
using Feats.Domain.Events;
using Feats.EventStore.Aggregates;

namespace Feats.Management.Features.Commands
{
    public sealed class UpdateFeatureCommand : ICommand
    {
        public string Name { get; set; }

        public string Path { get; set; }
        
        public string NewName { get; set; }

        public string NewPath { get; set; }
        
        public string UpdatedBy { get; set; }
    }

    public class UpdateFeatureCommandHandler : IHandleCommand<UpdateFeatureCommand>
    {
        private readonly ILogger _logger;

        private readonly IFeaturesAggregate _featuresAggregate;

        private readonly IPathsAggregate _pathsAggregate;

        private readonly ISystemClock _clock;

        public UpdateFeatureCommandHandler(
            ILogger<UpdateFeatureCommandHandler> logger,
            IFeaturesAggregate featuresAggregate,
            IPathsAggregate pathAggregate,
            ISystemClock clock)
        {
            this._logger = logger;
            this._featuresAggregate = featuresAggregate;
            this._pathsAggregate = pathAggregate;
            this._clock = clock;
        }

        public async Task Handle(UpdateFeatureCommand command)
        {
            command.Validate();
            await this._featuresAggregate.Load();
                       
            var FeatureUpdatedEvent = command
                .ExtractFeatureUpdatedEvent(this._clock);

            await this._featuresAggregate.Publish(FeatureUpdatedEvent);

            var pathRemovedEvent = command
                .ExtractPathRemovedEvent(this._clock);
            var pathCreatedEvent = command
                .ExtractPathCreatedEvent(this._clock);

            await this._pathsAggregate.Publish(pathRemovedEvent);
            await this._pathsAggregate.Publish(pathCreatedEvent);
        }
    }

    public static class UpdateFeatureCommandExtensions 
    {
        public static void Validate(this UpdateFeatureCommand command)
        {
            command.Required(nameof(command));
            command.Name.Required(nameof(command.Name));
            command.NewName.Required(nameof(command.NewName));
            command.UpdatedBy.Required(nameof(command.UpdatedBy));
        }

        public static FeatureUpdatedEvent ExtractFeatureUpdatedEvent(this UpdateFeatureCommand command, ISystemClock clock)
        {
            return new FeatureUpdatedEvent
            {
                Name = command.Name,
                Path = command.Path,
                NewName = command.NewName,
                NewPath = command.NewPath,
                UpdatedBy = command.UpdatedBy,
                UpdatedOn = clock.UtcNow,
            };
        }
        
        public static PathRemovedEvent ExtractPathRemovedEvent(this UpdateFeatureCommand command, ISystemClock clock)
        {
            return new PathRemovedEvent
            {
                RemovedBy = command.UpdatedBy,
                RemovedOn = clock.UtcNow,
                Path = command.Path,
                FeatureRemoved = command.Name,
            };
        }

        public static PathCreatedEvent ExtractPathCreatedEvent(this UpdateFeatureCommand command, ISystemClock clock)
        {
            return new PathCreatedEvent
            {
                CreatedBy = command.UpdatedBy,
                CreatedOn = clock.UtcNow,
                Path = command.NewPath,
                FeatureAdded = command.NewName,
            };
        }
    }
}
