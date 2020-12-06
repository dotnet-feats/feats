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
    public class CreateFeatureCommandHandlerTests : TestBase
    {
        [Test]
        public async Task GivenAnInvalidCommand_WhenCreatingAFeature_ThenWeThrow()
        {
            var featuresAggregate = this.GivenIFeaturesAggregate()
                .WithPublishing();
            var pathsAggregate = this.GivenIPathsAggregate()
                .WithPublishing();

            var command = new CreateFeatureCommand
            {
                CreatedBy = "meeee",
                Path = "let/me/show/you",
            };

            await this.GivenCommandHandler(featuresAggregate.Object, pathsAggregate.Object)
                .WhenCreatingAFeature(command)
                .ThenExceptionIsThrown<ArgumentValidationException>();
        }

        [Test]
        public async Task GivenACommand_WhenCreatingAFeatureThrowsInFeatures_ThenWeThrow()
        {
            var featuresAggregate = this.GivenIFeaturesAggregate()
                .WithPublishingThrows<FeatureAlreadyExistsException>();
            var pathsAggregate = this.GivenIPathsAggregate()
                .WithPublishing();

            var command = new CreateFeatureCommand
            {
                CreatedBy = "meeee",
                Name = "bob",
                Path = "let/me/show/you",
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

            var command = new CreateFeatureCommand
            {
                CreatedBy = "meeee",
                Name = "bob",
                Path = "let/me/show/you",
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

            var command = new CreateFeatureCommand
            {
                CreatedBy = "meeee",
                Name = "bob",
                Path = "let/me/show/you",
            };

            await this.GivenCommandHandler(featuresAggregate.Object, pathsAggregate.Object)
                .WhenCreatingAFeature(command)
                .ThenWePublish(featuresAggregate, pathsAggregate, command);
        }
    }

    public static class CreateFeatureCommandHandlerTestsExtensions
    {
        public static IHandleCommand<CreateFeatureCommand> GivenCommandHandler(
            this CreateFeatureCommandHandlerTests tests,
            IFeaturesAggregate featuresAggregate,
            IPathsAggregate pathsAggregate)
        {
            return new CreateFeatureCommandHandler(
                tests.GivenLogger<CreateFeatureCommandHandler>(),
                featuresAggregate,
                pathsAggregate,
                tests.GivenClock()
            );
        }

        public static Func<Task> WhenCreatingAFeature(
            this IHandleCommand<CreateFeatureCommand> handler,
            CreateFeatureCommand command)
        {
            return () => handler.Handle(command);
        }

        public static async Task ThenWePublish(
            this Func<Task> funk,
            Mock<IFeaturesAggregate> featuresAggregate,
            Mock<IPathsAggregate> pathsAggregate,
            CreateFeatureCommand command)
        {
            await funk();
            featuresAggregate.Verify(_ => _.Publish(
                It.Is<FeatureCreatedEvent>(e => e.Name.Equals(
                    command.Name, 
                    StringComparison.InvariantCultureIgnoreCase))), 
                Times.Once);
            pathsAggregate.Verify(_ => _.Publish(
                It.Is<PathCreatedEvent>(e => e.Path.Equals(
                    command.Path, 
                    StringComparison.InvariantCultureIgnoreCase))), 
                Times.Once);
        }
    }
}