using System;
using System.Threading.Tasks;
using Feats.Common.Tests;
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
    public class UnAssignStrategyCommandHandlerTests : TestBase
    {
        [Test]
        public async Task GivenAnInvalidCommand_WhenPublishingAFeature_ThenWeThrow()
        {
            var featuresAggregate = this
                .GivenIFeaturesAggregate()
                .WithPublishing();

            var command = new UnAssignStrategyCommand
            {
                UnassignedBy = "😎",
                Path = "🌲/🦝",
                StrategyName = "patate",
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
            
            var command = new UnAssignStrategyCommand
            {
                UnassignedBy = "😎",
                Name = "🌲/🦝",
                StrategyName = "charlie is depressed",
            };

            await this.GivenCommandHandler(featuresAggregate.Object)
                .WhenPublishingAFeature(command)
                .ThenExceptionIsThrown<NotImplementedException>();
        }

        [Test]
        public async Task GivenAValidCommand_WhenPublishingAFeature_ThenWePublishPathAndFeature()
        {
            var featuresAggregate = this.GivenIFeaturesAggregate();

            var command = new UnAssignStrategyCommand
            {
                UnassignedBy = "meeee",
                Name = "bob",
                Path = "let/me/show/you",
                StrategyName = "yay",
            };

            await this.GivenCommandHandler(featuresAggregate.Object)
                .WhenPublishingAFeature(command)
                .ThenWePublish(featuresAggregate, command);
        }
    }

    public static class UnassignStrategyCommandHandlerTestsExtensions
    {
        public static IHandleCommand<UnAssignStrategyCommand> GivenCommandHandler(
            this UnAssignStrategyCommandHandlerTests tests,
            IFeaturesAggregate featuresAggregate)
        {
            return new UnAssignStrategyCommandHandler(
                tests.GivenLogger<UnAssignStrategyCommandHandler>(),
                featuresAggregate,
                tests.GivenClock()
            );
        }

        public static Func<Task> WhenPublishingAFeature(
            this IHandleCommand<UnAssignStrategyCommand> handler,
            UnAssignStrategyCommand command)
        {
            return () => handler.Handle(command);
        }

        public static async Task ThenWePublish(
            this Func<Task> funk,
            Mock<IFeaturesAggregate> featuresAggregate,
            UnAssignStrategyCommand command)
        {
            await funk();
            featuresAggregate.Verify(_ => _.Publish(
                It.Is<StrategyUnAssignedEvent>(e => e.Name.Equals(
                    command.Name, 
                    StringComparison.InvariantCultureIgnoreCase))), 
                Times.Once);
        }
    }
}