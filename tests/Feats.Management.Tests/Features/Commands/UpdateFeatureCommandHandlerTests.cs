using System;
using System.Threading.Tasks;
using Feats.Common.Tests;
using Feats.CQRS.Commands;
using Feats.Domain;
using Feats.Domain.Events;
using Feats.EventStore.Aggregates;
using Feats.Management.Features.Commands;
using Feats.Management.Features.Exceptions;
using Feats.Management.Tests.Features.TestExtensions;
using Feats.Management.Tests.Paths.TestExtensions;
using Moq;
using NUnit.Framework;

namespace Feats.Management.Tests.Features.Commands
{
    public class UpdateFeatureCommandHandlerTests : TestBase
    {
        [Test]
        public async Task GivenAnInvalidCommand_WhenCreatingAFeature_ThenWeThrow()
        {
            var featuresAggregate = this.GivenIFeaturesAggregate()
                .WithPublishing();
            var pathsAggregate = this.GivenIPathsAggregate()
                .WithPublishing();

            var command = new UpdateFeatureCommand
            {
                UpdatedBy = "meeee",
                Path = "let/me/show/you",
            };

            await this.GivenCommandHandler(featuresAggregate.Object, pathsAggregate.Object)
                .WhenCreatingAFeature(command)
                .ThenExceptionIsThrown<ArgumentValidationException>();
        }

        [Test]
        public async Task GivenACommand_WhenCreatingAFeatureThrowsInFeaturesAggregate_ThenWeThrow()
        {
            var featuresAggregate = this.GivenIFeaturesAggregate()
                .WithPublishingThrows<FeatureAlreadyExistsException>();
            var pathsAggregate = this.GivenIPathsAggregate()
                .WithPublishing();

            var command = new UpdateFeatureCommand
            {
                UpdatedBy = "meeee",
                Name = "bob",
                Path = "let/me/show/you",
                NewName = "new",
                NewPath = "road",
            };

            await this.GivenCommandHandler(featuresAggregate.Object, pathsAggregate.Object)
                .WhenCreatingAFeature(command)
                .ThenExceptionIsThrown<FeatureAlreadyExistsException>();
        }
        
        [Test]
        public async Task GivenACommand_WhenCreatingAFeatureThrowsInPaths_ThenWeThrow()
        {
            var featuresAggregate = this.GivenIFeaturesAggregate()
                .WithPublishing();
            var pathsAggregate = this.GivenIPathsAggregate()
                .WithPublishingThrows<ArgumentNullException>();

            var command = new UpdateFeatureCommand
            {
                UpdatedBy = "meeee",
                Name = "bob",
                Path = "let/me/show/you",
                NewName = "new",
                NewPath = "road",
            };

            await this.GivenCommandHandler(featuresAggregate.Object, pathsAggregate.Object)
                .WhenCreatingAFeature(command)
                .ThenExceptionIsThrown<ArgumentNullException>();
        }

        [Test]
        public async Task GivenAValidCommand_WhenCreatingAFeature_ThenWePublishPathAndFeature()
        {
            var featuresAggregate = this.GivenIFeaturesAggregate();
            var pathsAggregate = this.GivenIPathsAggregate();

            var command = new UpdateFeatureCommand
            {
                UpdatedBy = "meeee",
                Name = "bob",
                Path = "let/me/show/you",
                NewName = "new",
                NewPath = "road",
            };

            await this.GivenCommandHandler(featuresAggregate.Object, pathsAggregate.Object)
                .WhenCreatingAFeature(command)
                .ThenWePublish(featuresAggregate, pathsAggregate, command);
        }
    }

    public static class UpdateFeatureCommandHandlerTestsExtensions
    {
        public static IHandleCommand<UpdateFeatureCommand> GivenCommandHandler(
            this UpdateFeatureCommandHandlerTests tests,
            IFeaturesAggregate featuresAggregate,
            IPathsAggregate pathsAggregate)
        {
            return new UpdateFeatureCommandHandler(
                tests.GivenLogger<UpdateFeatureCommandHandler>(),
                featuresAggregate,
                pathsAggregate,
                tests.GivenClock()
            );
        }

        public static Func<Task> WhenCreatingAFeature(
            this IHandleCommand<UpdateFeatureCommand> handler,
            UpdateFeatureCommand command)
        {
            return () => handler.Handle(command);
        }

        public static async Task ThenWePublish(
            this Func<Task> funk,
            Mock<IFeaturesAggregate> featuresAggregate,
            Mock<IPathsAggregate> pathsAggregate,
            UpdateFeatureCommand command)
        {
            await funk();
            featuresAggregate.Verify(_ => _.Publish(
                It.Is<FeatureUpdatedEvent>(e => e.NewName.Equals(
                    command.NewName, 
                    StringComparison.InvariantCultureIgnoreCase))), 
                Times.Once);
                
            pathsAggregate.Verify(_ => _.Publish(
                It.Is<PathRemovedEvent>(e => e.Path.Equals(
                    command.Path, 
                    StringComparison.InvariantCultureIgnoreCase))), 
                Times.Once);
            pathsAggregate.Verify(_ => _.Publish(
                It.Is<PathCreatedEvent>(e => e.Path.Equals(
                    command.NewPath, 
                    StringComparison.InvariantCultureIgnoreCase))), 
                Times.Once);
        }
    }
}