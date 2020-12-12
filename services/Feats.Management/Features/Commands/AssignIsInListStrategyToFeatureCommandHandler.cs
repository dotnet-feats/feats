using System;
using System.Threading.Tasks;

using Feats.CQRS.Commands;
using Feats.Domain.Validations;

using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Logging;
using Feats.Domain.Events;
using Feats.Domain.Strategies;
using Feats.EventStore.Aggregates;
using System.Collections.Generic;
using System.Linq;

namespace Feats.Management.Features.Commands
{
    public sealed class AssignIsInListStrategyToFeatureCommand : ICommand
    {
        public AssignIsInListStrategyToFeatureCommand()
        {
            this.Items = Enumerable.Empty<string>();
        }
        
        public string Name { get; set; }

        public string Path { get; set; }

        public string AssignedBy { get; set; }

        public IEnumerable<string> Items { get; set; }
    }

    public class AssignIsInListStrategyToFeatureCommandHandler : IHandleCommand<AssignIsInListStrategyToFeatureCommand>
    {
        private readonly IFeaturesAggregate _featuresAggregate;

        private readonly ISystemClock _clock;
        
        private readonly IStrategySettingsSerializer _serializer;

        public AssignIsInListStrategyToFeatureCommandHandler(
            IFeaturesAggregate featuresAggregate,
            ISystemClock clock,
            IStrategySettingsSerializer serializer)
        {
            this._featuresAggregate = featuresAggregate;
            this._clock = clock;
            this._serializer = serializer;
        }

        public async Task Handle(AssignIsInListStrategyToFeatureCommand command)
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

    public static class AssignIsInListStrategyToFeatureCommandExtensions 
    {
        public static void Validate(this AssignIsInListStrategyToFeatureCommand command)
        {
            command.Required(nameof(command));
            command.Name.Required(nameof(command.Name));
            command.AssignedBy.Required(nameof(command.AssignedBy));
        }

        public static StrategyAssignedEvent ExtractStrategyAssignedEvent(
            this AssignIsInListStrategyToFeatureCommand command, 
            ISystemClock clock,
            IStrategySettingsSerializer serializer)
        {
            return new StrategyAssignedEvent
            {
                Name = command.Name,
                Path = command.Path,
                AssignedBy = command.AssignedBy,
                AssignedOn = clock.UtcNow,
                StrategyName = StrategyNames.IsInList,
                Settings = serializer.Serialize(new IsInListStrategySettings 
                {
                    Items = command.Items,
                })
            };
        }
    }
}
