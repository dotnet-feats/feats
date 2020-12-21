using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Feats.Common.Tests;
using Feats.Common.Tests.Strategies.TestExtensions;
using Feats.CQRS.Commands;
using Feats.Domain.Aggregates;
using Feats.Domain.Events;
using Feats.Domain.Exceptions;
using Feats.Management.Features.Commands;
using Feats.Management.Tests.Features.TestExtensions;
using Moq;
using NUnit.Framework;

namespace Feats.Management.Tests.Features.Commands
{
    public class AssignIsLowerThanStrategyToFeatureCommandHandlerTests : TestBase
    {
        [Test]
        public async Task GivenAnInvalidCommand_WhenPublishingAFeature_ThenWeThrow()
        {
            var featuresAggregate = this
                .GivenIFeaturesAggregate()
                .WithPublishing();

            var command = new AssignIsLowerThanStrategyToFeatureCommand
            {
                AssignedBy = "😎",
                Path = "🌲/🦝"
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
            
            var command = new AssignIsLowerThanStrategyToFeatureCommand
            {
                AssignedBy = "😎",
                Name = "🌲/🦝",
                Value = 91L
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

            var command = new AssignIsLowerThanStrategyToFeatureCommand
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

            var command = new AssignIsLowerThanStrategyToFeatureCommand
            {
                AssignedBy = "meeee",
                Name = "bob",
                Path = "let/me/show/you",
                Value = 09
            };

            await this.GivenCommandHandler(featuresAggregate.Object)
                .WhenPublishingAFeature(command)
                .ThenWePublish(featuresAggregate, command);
        }
    }

    public static class AssignIsLowerThanStrategyToFeatureCommandHandlerTestsExtensions
    {
        public static IHandleCommand<AssignIsLowerThanStrategyToFeatureCommand> GivenCommandHandler(
            this AssignIsLowerThanStrategyToFeatureCommandHandlerTests tests,
            IFeaturesAggregate featuresAggregate)
        {
            return new AssignIsLowerThanStrategyToFeatureCommandHandler(
                featuresAggregate,
                tests.GivenClock(),
                tests.GivenIStrategySettingsSerializer()
            );
        }

        public static Func<Task> WhenPublishingAFeature(
            this IHandleCommand<AssignIsLowerThanStrategyToFeatureCommand> handler,
            AssignIsLowerThanStrategyToFeatureCommand command)
        {
            return () => handler.Handle(command);
        }

        public static async Task ThenWePublish(
            this Func<Task> funk,
            Mock<IFeaturesAggregate> featuresAggregate,
            AssignIsLowerThanStrategyToFeatureCommand command)
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