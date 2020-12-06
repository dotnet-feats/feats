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
    public class UnassignStrategyCommandHandlerTests : TestBase
    {
        [Test]
        public async Task GivenAnInvalidCommand_WhenPublishingAFeature_ThenWeThrow()
        {
            var featuresAggregate = this
                .GivenIFeaturesAggregate()
                .WithPublishing();

            var command = new UnassignStrategyCommand
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
            
            var command = new UnassignStrategyCommand
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

            var command = new UnassignStrategyCommand
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
        public static IHandleCommand<UnassignStrategyCommand> GivenCommandHandler(
            this UnassignStrategyCommandHandlerTests tests,
            IFeaturesAggregate featuresAggregate)
        {
            return new UnassignStrategyCommandHandler(
                tests.GivenLogger<UnassignStrategyCommandHandler>(),
                featuresAggregate,
                tests.GivenClock()
            );
        }

        public static Func<Task> WhenPublishingAFeature(
            this IHandleCommand<UnassignStrategyCommand> handler,
            UnassignStrategyCommand command)
        {
            return () => handler.Handle(command);
        }

        public static async Task ThenWePublish(
            this Func<Task> funk,
            Mock<IFeaturesAggregate> featuresAggregate,
            UnassignStrategyCommand command)
        {
            await funk();
            featuresAggregate.Verify(_ => _.Publish(
                It.Is<StrategyUnassignedEvent>(e => e.Name.Equals(
                    command.Name, 
                    StringComparison.InvariantCultureIgnoreCase))), 
                Times.Once);
        }
    }
}