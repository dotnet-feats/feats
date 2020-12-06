using System;
using System.Threading.Tasks;

using Feats.CQRS.Commands;
using Feats.Domain.Events;
using Feats.Domain.Validations;
using Feats.EventStore.Aggregates;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Logging;

namespace Feats.Management.Features.Commands
{
    public sealed class PublishFeatureCommand : ICommand
    {
        public string Name { get; set; }

        public string Path { get; set; }

        public string PublishedBy { get; set; }
    }

    public class PublishFeatureCommandHandler : IHandleCommand<PublishFeatureCommand>
    {
        private readonly ILogger _logger;

        private readonly IFeaturesAggregate _featuresAggregate;

        private readonly ISystemClock _clock;

        public PublishFeatureCommandHandler(
            ILogger<PublishFeatureCommandHandler> logger,
            IFeaturesAggregate featuresAggregate,
            ISystemClock clock)
        {
            this._logger = logger;
            this._featuresAggregate = featuresAggregate;
            this._clock = clock;
        }

        public async Task Handle(PublishFeatureCommand command)
        {
            command.Validate();
            await this._featuresAggregate.Load();
                       
            var publishedEvent = command
                .ExtractFeaturePublishedEvent(this._clock);

            await this._featuresAggregate.Publish(publishedEvent);
        }
    }

    public static class PublishFeatureCommandExtensions 
    {
        public static void Validate(this PublishFeatureCommand command)
        {
            command.Required(nameof(command));
            command.Name.Required(nameof(command.Name));
            command.PublishedBy.Required(nameof(command.PublishedBy));
        }

        public static FeaturePublishedEvent ExtractFeaturePublishedEvent(this PublishFeatureCommand command, ISystemClock clock)
        {
            return new FeaturePublishedEvent
            {
                Name = command.Name,
                PublishedBy = command.PublishedBy,
                PublishedOn = clock.UtcNow,
                Path = command.Path,
            };
        }
    }
}
