using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Feats.Common.Tests;
using Feats.Common.Tests.Strategies.TestExtensions;
using Feats.CQRS.Commands;
using Feats.Domain;
using Feats.Domain.Events;
using Feats.Domain.Exceptions;
using Feats.EventStore.Aggregates;
using Feats.Management.Features.Commands;
using Feats.Management.Tests.Features.TestExtensions;
using Moq;
using NUnit.Framework;

namespace Feats.Management.Tests.Features.Commands
{
    public class AssignIsInListStrategyToFeatureCommandHandlerTests : TestBase
    {
        [Test]
        public async Task GivenAnInvalidCommand_WhenPublishingAFeature_ThenWeThrow()
        {
            var featuresAggregate = this
                .GivenIFeaturesAggregate()
                .WithPublishing();

            var command = new AssignIsInListStrategyToFeatureCommand
            {
                AssignedBy = "😎",
                Path = "🌲/🦝",
                Items = new List<string> { "a" },
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
            
            var command = new AssignIsInListStrategyToFeatureCommand
            {
                AssignedBy = "😎",
                Name = "🌲/🦝",
                Items = new List<string> { "a" },
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

            var command = new AssignIsInListStrategyToFeatureCommand
            {
                AssignedBy = "meeee",
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

            var command = new AssignIsInListStrategyToFeatureCommand
            {
                AssignedBy = "meeee",
                Name = "bob",
                Path = "let/me/show/you",
                Items = new List<string> { "a" },
            };

            await this.GivenCommandHandler(featuresAggregate.Object)
                .WhenPublishingAFeature(command)
                .ThenWePublish(featuresAggregate, command);
        }
    }

    public static class AssignIsInListStrategyToFeatureCommandHandlerTestsExtensions
    {
        public static IHandleCommand<AssignIsInListStrategyToFeatureCommand> GivenCommandHandler(
            this AssignIsInListStrategyToFeatureCommandHandlerTests tests,
            IFeaturesAggregate featuresAggregate)
        {
            return new AssignIsInListStrategyToFeatureCommandHandler(
                tests.GivenLogger<AssignIsInListStrategyToFeatureCommandHandler>(),
                featuresAggregate,
                tests.GivenClock(),
                tests.GivenIStrategySettingsSerializer()
            );
        }

        public static Func<Task> WhenPublishingAFeature(
            this IHandleCommand<AssignIsInListStrategyToFeatureCommand> handler,
            AssignIsInListStrategyToFeatureCommand command)
        {
            return () => handler.Handle(command);
        }

        public static async Task ThenWePublish(
            this Func<Task> funk,
            Mock<IFeaturesAggregate> featuresAggregate,
            AssignIsInListStrategyToFeatureCommand command)
        {
            await funk();
            featuresAggregate.Verify(_ => _.Publish(
                It.Is<StrategyAssignedEvent>(e => e.Name.Equals(
                    command.Name, 
                    StringComparison.InvariantCultureIgnoreCase))), 
                Times.Once);
        }
    }
}