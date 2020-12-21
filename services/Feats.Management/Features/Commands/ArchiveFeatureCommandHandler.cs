using System;
using System.Threading.Tasks;

using Feats.CQRS.Commands;
using Feats.Domain.Aggregates;
using Feats.Domain.Events;
using Feats.Domain.Validations;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Logging;

namespace Feats.Management.Features.Commands
{
    public sealed class ArchiveFeatureCommand : ICommand
    {
        public string Name { get; set; }

        public string Path { get; set; }

        public string ArchivedBy { get; set; }
    }

    public class ArchiveFeatureCommandHandler : IHandleCommand<ArchiveFeatureCommand>
    {
        private readonly ILogger _logger;

        private readonly IFeaturesAggregate _featuresAggregate;

        private readonly ISystemClock _clock;

        public ArchiveFeatureCommandHandler(
            ILogger<ArchiveFeatureCommandHandler> logger,
            IFeaturesAggregate featuresAggregate,
            ISystemClock clock)
        {
            this._logger = logger;
            this._featuresAggregate = featuresAggregate;
            this._clock = clock;
        }

        public async Task Handle(ArchiveFeatureCommand command)
        {
            command.Validate();
            await this._featuresAggregate.Load();
                       
            var archivedEvent = command
                .ExtractFeatureArchivedEvent(this._clock);

            await this._featuresAggregate.Publish(archivedEvent);
        }
    }

    public static class ArchiveFeatureCommandExtensions 
    {
        public static void Validate(this ArchiveFeatureCommand command)
        {
            command.Required(nameof(command));
            command.Name.Required(nameof(command.Name));
            command.ArchivedBy.Required(nameof(command.ArchivedBy));
        }

        public static FeatureArchivedEvent ExtractFeatureArchivedEvent(this ArchiveFeatureCommand command, ISystemClock clock)
        {
            return new FeatureArchivedEvent
            {
                Name = command.Name,
                ArchivedBy = command.ArchivedBy,
                ArchivedOn = clock.UtcNow,
                Path = command.Path,
            };
        }
    }
}
