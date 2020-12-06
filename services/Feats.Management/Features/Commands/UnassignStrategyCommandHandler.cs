using System;
using System.Threading.Tasks;

using Feats.CQRS.Commands;
using Feats.Domain.Validations;

using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Logging;
using Feats.Domain.Events;
using Feats.Domain.Strategies;
using Feats.EventStore.Aggregates;

namespace Feats.Management.Features.Commands
{
    public sealed class UnassignStrategyCommand : ICommand
    {
        public string Name { get; set; }

        public string Path { get; set; }

        public string UnassignedBy { get; set; }

        public string StrategyName { get; set; }
    }

    public class UnassignStrategyCommandHandler : IHandleCommand<UnassignStrategyCommand>
    {
        private readonly ILogger _logger;

        private readonly IFeaturesAggregate _featuresAggregate;

        private readonly ISystemClock _clock;
        private readonly IStrategySettingsSerializer _serializer;

        public UnassignStrategyCommandHandler(
            ILogger<UnassignStrategyCommandHandler> logger,
            IFeaturesAggregate featuresAggregate,
            ISystemClock clock)
        {
            this._logger = logger;
            this._featuresAggregate = featuresAggregate;
            this._clock = clock;
        }

        public async Task Handle(UnassignStrategyCommand command)
        {
            command.Validate();
            await this._featuresAggregate.Load();
                       
            var @event = command
                .ExtractStrategyUnassignedEvent(
                    this._clock);

            await this._featuresAggregate.Publish(@event);
        }
    }

    public static class UnassignStrategyCommandExtensions 
    {
        public static void Validate(this UnassignStrategyCommand command)
        {
            command.Required(nameof(command));
            command.Name.Required(nameof(command.Name));
            command.StrategyName.Required(nameof(command.StrategyName));
            command.UnassignedBy.Required(nameof(command.UnassignedBy));
        }

        public static StrategyUnassignedEvent ExtractStrategyUnassignedEvent(
            this UnassignStrategyCommand command, 
            ISystemClock clock)
        {
            return new StrategyUnassignedEvent
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
