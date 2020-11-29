using System;
using System.Threading.Tasks;

using Feats.CQRS.Commands;
using Feats.Domain.Validations;

using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Logging;
using Feats.Domain.Events;
using Feats.Domain.Strategies;
using System.Text.Json;

namespace Feats.Management.Features.Commands
{
    public sealed class AssignIsOnStrategyToFeatureCommand : ICommand
    {
        public string Name { get; set; }

        public string Path { get; set; }

        public string AssignedBy { get; set; }

        public bool IsEnabled { get; set; }
    }

    public class AssignIsOnStrategyToFeatureCommandHandler : IHandleCommand<AssignIsOnStrategyToFeatureCommand>
    {
        private readonly ILogger _logger;

        private readonly IFeaturesAggregate _featuresAggregate;

        private readonly ISystemClock _clock;

        public AssignIsOnStrategyToFeatureCommandHandler(
            ILogger<AssignIsOnStrategyToFeatureCommandHandler> logger,
            IFeaturesAggregate featuresAggregate,
            ISystemClock clock)
        {
            this._logger = logger;
            this._featuresAggregate = featuresAggregate;
            this._clock = clock;
        }

        public async Task Handle(AssignIsOnStrategyToFeatureCommand command)
        {
            command.Validate();
            await this._featuresAggregate.Load();
                       
            var @event = command
                .ExtractStrategyAssignedEvent(this._clock);

            await this._featuresAggregate.Publish(@event);
        }
    }

    public static class AssignIsOnStrategyToFeatureCommandExtensions 
    {
        public static void Validate(this AssignIsOnStrategyToFeatureCommand command)
        {
            command.Required(nameof(command));
            command.Name.Required(nameof(command.Name));
            command.AssignedBy.Required(nameof(command.AssignedBy));
        }

        public static StrategyAssignedEvent ExtractStrategyAssignedEvent(this AssignIsOnStrategyToFeatureCommand command, ISystemClock clock)
        {
            return new StrategyAssignedEvent
            {
                Name = command.Name,
                Path = command.Path,
                AssignedBy = command.AssignedBy,
                AssignedOn = clock.UtcNow,
                StrategyName = StrategyNames.IsEnabled,
                Settings = JsonSerializer.Serialize(new IsEnabledStrategySettings 
                {
                    IsOn = command.IsEnabled,
                })
            };
        }
    }
}
