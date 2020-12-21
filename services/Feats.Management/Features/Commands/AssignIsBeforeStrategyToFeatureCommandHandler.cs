using System;
using System.Threading.Tasks;

using Feats.CQRS.Commands;
using Feats.Domain.Validations;

using Microsoft.Extensions.Internal;
using Feats.Domain.Events;
using Feats.Domain.Strategies;
using Feats.Domain.Aggregates;

namespace Feats.Management.Features.Commands
{
    public sealed class AssignIsBeforeStrategyToFeatureCommand : ICommand
    {
        public string Name { get; set; }

        public string Path { get; set; }

        public string AssignedBy { get; set; }

        public DateTimeOffset Value { get; set; }
    }

    public class AssignIsBeforeStrategyToFeatureCommandHandler : IHandleCommand<AssignIsBeforeStrategyToFeatureCommand>
    {
        private readonly IFeaturesAggregate _featuresAggregate;

        private readonly ISystemClock _clock;
        
        private readonly IStrategySettingsSerializer _serializer;

        public AssignIsBeforeStrategyToFeatureCommandHandler(
            IFeaturesAggregate featuresAggregate,
            ISystemClock clock,
            IStrategySettingsSerializer serializer)
        {
            this._featuresAggregate = featuresAggregate;
            this._clock = clock;
            this._serializer = serializer;
        }

        public async Task Handle(AssignIsBeforeStrategyToFeatureCommand command)
        {
            command.Validate();
            await this._featuresAggregate.Load();
                       
            var @event = command
                .ExtractStrategyAssignedEvent(
                    this._clock,
                    this._serializer);

            await this._featuresAggregate.Publish(@event);
        }
    }

    public static class AssignIsBeforeStrategyToFeatureCommandExtensions 
    {
        public static void Validate(this AssignIsBeforeStrategyToFeatureCommand command)
        {
            command.Required(nameof(command));
            command.Name.Required(nameof(command.Name));
            command.Value.Required(nameof(command.Value));
            command.AssignedBy.Required(nameof(command.AssignedBy));
        }

        public static StrategyAssignedEvent ExtractStrategyAssignedEvent(
            this AssignIsBeforeStrategyToFeatureCommand command, 
            ISystemClock clock,
            IStrategySettingsSerializer serializer)
        {
            return new StrategyAssignedEvent
            {
                Name = command.Name,
                Path = command.Path,
                AssignedBy = command.AssignedBy,
                AssignedOn = clock.UtcNow,
                StrategyName = StrategyNames.IsBefore,
                Settings = serializer.Serialize(new DateTimeOffsetStrategySettings 
                {
                    Value = command.Value
                })
            };
        }
    }
}
