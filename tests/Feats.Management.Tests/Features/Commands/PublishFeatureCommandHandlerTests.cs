using System;
using System.Threading.Tasks;
using Feats.Common.Tests;
using Feats.CQRS.Commands;
using Feats.Domain;
using Feats.Domain.Events;
using Feats.EventStore.Aggregates;
using Feats.Management.Features.Commands;
using Feats.Management.Tests.Features.TestExtensions;
using Feats.Management.Tests.Paths.TestExtensions;
using Moq;
using NUnit.Framework;

namespace Feats.Management.Tests.Features.Commands
{
    public class PublishFeatureCommandHandlerTests : TestBase
    {
        [Test]
        public async Task GivenAnInvalidCommand_WhenPublishingAFeature_ThenWeThrow()
        {
            var featuresAggregate = this.GivenIFeaturesAggregate()
                .WithPublishing();

            var command = new PublishFeatureCommand
            {
                PublishedBy = "😎",
                Path = "🌲/🦝",
            };

            await this.GivenCommandHandler(featuresAggregate.Object)
                .WhenPublishingAFeature(command)
                .ThenExceptionIsThrown<ArgumentValidationException>();
        }

        [Test]
        public async Task GivenACommand_WhenPublishingThrows_ThenWeThrow()
        {
            var featuresAggregate = this.GivenIFeaturesAggregate()
                .WithPublishingThrows<NotImplementedException>();
            
            var command = new PublishFeatureCommand
            {
                PublishedBy = "😎",
                Name = "🌲/🦝",
            };

            await this.GivenCommandHandler(featuresAggregate.Object)
                .WhenPublishingAFeature(command)
                .ThenExceptionIsThrown<NotImplementedException>();
        }

        [Test]
        public async Task GivenACommand_WhenPublishingAFeatureThatDoesNotExist_ThenWeDoNotThrow()
        {
            var featuresAggregate = this.GivenIFeaturesAggregate()
                .WithPublishing();

            var command = new PublishFeatureCommand
            {
                PublishedBy = "meeee",
                Name = "bob",
                Path = "let/me/show/you",
            };

            await this.GivenCommandHandler(featuresAggregate.Object)
                .WhenPublishingAFeature(command)
                .ThenWePublish(featuresAggregate, command);
        }

        [Test]
        public async Task GivenAValidCommand_WhenPublishingAFeature_ThenWePublishPathAndFeature()
        {
            var featuresAggregate = this.GivenIFeaturesAggregate();

            var command = new PublishFeatureCommand
            {
                PublishedBy = "meeee",
                Name = "bob",
                Path = "let/me/show/you",
            };

            await this.GivenCommandHandler(featuresAggregate.Object)
                .WhenPublishingAFeature(command)
                .ThenWePublish(featuresAggregate, command);
        }
    }

    public static class PublishFeatureCommandHandlerTestsExtensions
    {
        public static IHandleCommand<PublishFeatureCommand> GivenCommandHandler(
            this PublishFeatureCommandHandlerTests tests,
            IFeaturesAggregate featuresAggregate)
        {
            return new PublishFeatureCommandHandler(
                tests.GivenLogger<PublishFeatureCommandHandler>(),
                featuresAggregate,
                tests.GivenClock()
            );
        }

        public static Func<Task> WhenPublishingAFeature(
            this IHandleCommand<PublishFeatureCommand> handler,
            PublishFeatureCommand command)
        {
            return () => handler.Handle(command);
        }

        public static async Task ThenWePublish(
            this Func<Task> funk,
            Mock<IFeaturesAggregate> featuresAggregate,
            PublishFeatureCommand command)
        {
            await funk();
            featuresAggregate.Verify(_ => _.Publish(
                It.Is<FeaturePublishedEvent>(e => e.Name.Equals(
                    command.Name, 
                    StringComparison.InvariantCultureIgnoreCase))), 
                Times.Once);
        }

        public static async Task ThenWeDontPublish(
            this Func<Task> funk,
            Mock<IFeaturesAggregate> featuresAggregate,
            PublishFeatureCommand command)
        {
            await funk();
            featuresAggregate.Verify(_ => _.Publish(
                It.Is<FeaturePublishedEvent>(e => e.Name.Equals(
                    command.Name, 
                    StringComparison.InvariantCultureIgnoreCase))), 
                Times.Never);
        }
    }
}