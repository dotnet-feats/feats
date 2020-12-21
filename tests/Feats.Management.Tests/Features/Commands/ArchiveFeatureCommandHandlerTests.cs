using System;
using System.Threading.Tasks;
using Feats.Common.Tests;
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
    public class ArchiveFeatureCommandHandlerTests : TestBase
    {
        [Test]
        public async Task GivenAnInvalidCommand_WhenArchiveingAFeature_ThenWeThrow()
        {
            var featuresAggregate = this.GivenIFeaturesAggregate()
                .WithPublishing();

            var command = new ArchiveFeatureCommand
            {
                ArchivedBy = "😎",
                Path = "🌲/🦝",
            };

            await this.GivenCommandHandler(featuresAggregate.Object)
                .WhenArchiveingAFeature(command)
                .ThenExceptionIsThrown<ArgumentValidationException>();
        }

        [Test]
        public async Task GivenACommand_WhenArchiveingThrows_ThenWeThrow()
        {
            var featuresAggregate = this.GivenIFeaturesAggregate()
                .WithPublishingThrows<NotImplementedException>();
            
            var command = new ArchiveFeatureCommand
            {
                ArchivedBy = "😎",
                Name = "🌲/🦝",
            };

            await this.GivenCommandHandler(featuresAggregate.Object)
                .WhenArchiveingAFeature(command)
                .ThenExceptionIsThrown<NotImplementedException>();
        }

        [Test]
        public async Task GivenACommand_WhenArchiveingAFeatureThatDoesNotExist_ThenWeDoNotThrow()
        {
            var featuresAggregate = this.GivenIFeaturesAggregate()
                .WithPublishing();

            var command = new ArchiveFeatureCommand
            {
                ArchivedBy = "meeee",
                Name = "bob",
                Path = "let/me/show/you",
            };

            await this.GivenCommandHandler(featuresAggregate.Object)
                .WhenArchiveingAFeature(command)
                .ThenWeArchive(featuresAggregate, command);
        }

        [Test]
        public async Task GivenAValidCommand_WhenArchiveingAFeature_ThenWeArchivePathAndFeature()
        {
            var featuresAggregate = this.GivenIFeaturesAggregate();

            var command = new ArchiveFeatureCommand
            {
                ArchivedBy = "meeee",
                Name = "bob",
                Path = "let/me/show/you",
            };

            await this.GivenCommandHandler(featuresAggregate.Object)
                .WhenArchiveingAFeature(command)
                .ThenWeArchive(featuresAggregate, command);
        }
    }

    public static class ArchiveFeatureCommandHandlerTestsExtensions
    {
        public static IHandleCommand<ArchiveFeatureCommand> GivenCommandHandler(
            this ArchiveFeatureCommandHandlerTests tests,
            IFeaturesAggregate featuresAggregate)
        {
            return new ArchiveFeatureCommandHandler(
                tests.GivenLogger<ArchiveFeatureCommandHandler>(),
                featuresAggregate,
                tests.GivenClock()
            );
        }

        public static Func<Task> WhenArchiveingAFeature(
            this IHandleCommand<ArchiveFeatureCommand> handler,
            ArchiveFeatureCommand command)
        {
            return () => handler.Handle(command);
        }

        public static async Task ThenWeArchive(
            this Func<Task> funk,
            Mock<IFeaturesAggregate> featuresAggregate,
            ArchiveFeatureCommand command)
        {
            await funk();
            featuresAggregate.Verify(_ => _.Publish(
                It.Is<FeatureArchivedEvent>(e => e.Name.Equals(
                    command.Name, 
                    StringComparison.InvariantCultureIgnoreCase))), 
                Times.Once);
        }

        public static async Task ThenWeDontArchive(
            this Func<Task> funk,
            Mock<IFeaturesAggregate> featuresAggregate,
            ArchiveFeatureCommand command)
        {
            await funk();
            featuresAggregate.Verify(_ => _.Publish(
                It.Is<FeatureArchivedEvent>(e => e.Name.Equals(
                    command.Name, 
                    StringComparison.InvariantCultureIgnoreCase))), 
                Times.Never);
        }
    }
}