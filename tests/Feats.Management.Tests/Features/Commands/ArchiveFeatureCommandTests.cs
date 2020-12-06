
using System;
using Feats.Common.Tests;
using Feats.Domain;
using Feats.Domain.Events;
using Feats.Management.Features.Commands;
using FluentAssertions;
using Microsoft.Extensions.Internal;
using NUnit.Framework;

namespace Feats.Management.Tests.Features.Commands
{
    public class ArchiveFeatureCommandTests : TestBase
    {
        [Test]
        public void GivenACommandWithAllSettings_WhenValidating_ThenNoExceptionIsThrown()
        {
            this.GivenValidCommand()
                .WhenValidating()
                .ThenNoExceptionIsThrown();
        }

        [Test]
        public void GivenACommandIsNUll_WhenValidating_ThenArgumentNullIsThrown()
        {
            ArchiveFeatureCommand command = null;

            command
                .WhenValidating()
                .ThenExceptionIsThrown<ArgumentValidationException>();
        }

        [Test]
        public void GivenACommandWithMissingName_WhenValidating_ThenArgumentNullIsThrown()
        {
            var command = this
                .GivenValidCommand();
            command.Name = string.Empty;

            command
                .WhenValidating()
                .ThenExceptionIsThrown<ArgumentValidationException>();
        }

        [Test]
        public void GivenACommandWithMissingPath_WhenValidating_ThenNoExceptionIsThrown()
        {
            var command = this
                .GivenValidCommand();
            command.Path = string.Empty;

            command
                .WhenValidating()
                .ThenNoExceptionIsThrown();
        }

        [Test]
        public void GivenACommandWithMissingCreatedBy_WhenValidating_ThenArgumentNullIsThrown()
        {
            var command = this
                .GivenValidCommand();
            command.ArchivedBy = string.Empty;

            command
                .WhenValidating()
                .ThenExceptionIsThrown<ArgumentValidationException>();
        }
        
        [Test]
        public void GivenACommand_WhenExtractingFeatureArchivedEvent_ThenNoExceptionIsThrownt()
        {
            var clock = this.GivenClock();
            
            this.GivenValidCommand()
                .WhenExtractingFeatureArchivedEvent(clock)
                .ThenNoExceptionIsThrown();
        }
        
        [Test]
        public void GivenACommand_WhenExtractingFeatureArchivedEvent_ThenWeGetAPathCreatedEvent()
        {
            var clock = this.GivenClock();
            var request = this
                .GivenValidCommand();
            
            request
                .WhenExtractingFeatureArchivedEvent(clock)
                .ThenWeGetAFeatureArchivedEvent(request, clock);
        }
    }

    public static class ArchiveFeatureCommandTestsExtensions 
    {
        public static ArchiveFeatureCommand GivenValidCommand(this ArchiveFeatureCommandTests tests)
        {
            return new ArchiveFeatureCommand
            {
                ArchivedBy = "🦄",
                Name = "bob ross 🎨🖌🖼",
                Path = "painting/in/winter",
            };
        }

        public static Action WhenValidating(this ArchiveFeatureCommand command)
        {
            return () => command.Validate();
        }

        public static Func<FeatureArchivedEvent> WhenExtractingFeatureArchivedEvent(this ArchiveFeatureCommand command, ISystemClock clock)
        {
            return () => command.ExtractFeatureArchivedEvent(clock);
        }
        
        public static void ThenWeGetAFeatureArchivedEvent(this Func<FeatureArchivedEvent> featFunc, ArchiveFeatureCommand command, ISystemClock clock)
        {
            var feat = featFunc();

            feat.ArchivedBy.Should().Be(command.ArchivedBy);
            feat.ArchivedOn.Should().Be(clock.UtcNow);
            feat.Name.Should().Be(command.Name);
            feat.Path.Should().Be(command.Path);
        }
    }
}