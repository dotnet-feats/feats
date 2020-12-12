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
    public sealed class UnAssignStrategyCommand : ICommand
    {
        public string Name { get; set; }

        public string Path { get; set; }

        public string UnassignedBy { get; set; }

        public string StrategyName { get; set; }
    }

    public class UnAssignStrategyCommandHandler : IHandleCommand<UnAssignStrategyCommand>
    {
        private readonly ILogger _logger;

        private readonly IFeaturesAggregate _featuresAggregate;

        private readonly ISystemClock _clock;

        public UnAssignStrategyCommandHandler(
            ILogger<UnAssignStrategyCommandHandler> logger,
            IFeaturesAggregate featuresAggregate,
            ISystemClock clock)
        {
            this._logger = logger;
            this._featuresAggregate = featuresAggregate;
            this._clock = clock;
        }

        public async Task Handle(UnAssignStrategyCommand command)
        {
            command.Validate();
            await this._featuresAggregate.Load();
                       
            var @event = command
                .ExtractStrategyUnassignedEvent(
                    this._clock);

            await this._featuresAggregate.Publish(@event);
        }
    }

    public static class UnAssignStrategyCommandExtensions 
    {
        public static void Validate(this UnAssignStrategyCommand command)
        {
            command.Required(nameof(command));
            command.Name.Required(nameof(command.Name));
            command.StrategyName.Required(nameof(command.StrategyName));
            command.UnassignedBy.Required(nameof(command.UnassignedBy));
        }

        public static StrategyUnAssignedEvent ExtractStrategyUnassignedEvent(
            this UnAssignStrategyCommand command, 
            ISystemClock clock)
        {
            return new StrategyUnAssignedEvent
            {
                Name = command.Name,
                Path = command.Path,
                UnassignedBy = command.UnassignedBy,
                UnassignedOn = clock.UtcNow,
                StrategyName = command.StrategyName
            };
        }
    }
}
