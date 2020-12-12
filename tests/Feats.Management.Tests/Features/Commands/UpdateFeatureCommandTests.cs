
using System;
using Feats.Common.Tests;
using Feats.Domain;
using Feats.Domain.Events;
using Feats.Domain.Exceptions;
using Feats.Management.Features.Commands;
using FluentAssertions;
using Microsoft.Extensions.Internal;
using NUnit.Framework;

namespace Feats.Management.Tests.Features.Commands
{
    public class UpdateFeatureCommandTests : TestBase
    {
        [Test]
        public void GivenACommandWithAllSettings_WhenValidating_ThenNoExceptionIsThrown()
        {
            this.GivenValidCommand()
                .WhenValidating()
                .ThenNoExceptionIsThrown();
        }

        [Test]
        public void GivenACommandIsNUll_WhenValidating_ThenWeThrow()
        {
            UpdateFeatureCommand command = null;

            command
                .WhenValidating()
                .ThenExceptionIsThrown<ArgumentValidationException>();
        }

        [Test]
        public void GivenACommandWithMissingName_WhenValidating_ThenWeThrow()
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
        public void GivenACommandWithMissinNewName_WhenValidating_ThenWeThrow()
        {
            var command = this
                .GivenValidCommand();
            command.NewName = string.Empty;

            command
                .WhenValidating()
                .ThenExceptionIsThrown<ArgumentValidationException>();
        }

        [Test]
        public void GivenACommandWithMissingNewPath_WhenValidating_ThenNoExceptionIsThrown()
        {
            var command = this
                .GivenValidCommand();
            command.NewPath = string.Empty;

            command
                .WhenValidating()
                .ThenNoExceptionIsThrown();
        }
        
        [Test]
        public void GivenACommandWithMissingUpdatedBy_WhenValidating_ThenWeThrow()
        {
            var command = this
                .GivenValidCommand();
            command.UpdatedBy = string.Empty;

            command
                .WhenValidating()
                .ThenExceptionIsThrown<ArgumentValidationException>();
        }
        
        [Test]
        public void GivenACommand_WhenExtractingFeatureUpdatedEvent_ThenWeGetAPathUpdatedEvent()
        {
            var clock = this.GivenClock();
            var request = this
                .GivenValidCommand();
            
            request
                .WhenExtractingFeatureUpdatedEvent(clock)
                .ThenWeGetAFeatureUpdatedEvent(request, clock);
        } 
        
        [Test]
        public void GivenACommand_WhenExtractingPathCreateddEvent_ThenWeGetAPathCreatedEvent()
        {
            var clock = this.GivenClock();
            var request = this
                .GivenValidCommand();
            
            request
                .WhenExtractingPathCreatedEvent(clock)
                .ThenWeGetAPathCreatedEvent(request, clock);
        }   
        
        [Test]
        public void GivenACommand_WhenExtractingPathRemovedEvent_ThenWeGetAPathRemovedEvent()
        {
            var clock = this.GivenClock();
            var request = this
                .GivenValidCommand();
            
            request
                .WhenExtractingPathRemovedEvent(clock)
                .ThenWeGetAPathRemovedEvent(request, clock);
        }         
    }

    public static class UpdateFeatureCommandTestsExtensions 
    {
        public static UpdateFeatureCommand GivenValidCommand(this UpdateFeatureCommandTests tests)
        {
            return new UpdateFeatureCommand
            {
                UpdatedBy = "🦄",
                Name = "bob ross 🎨🖌🖼",
                Path = "painting/in/winter",
                NewName = "new",
                NewPath = "because/reasons",
            };
        }

        public static Action WhenValidating(this UpdateFeatureCommand command)
        {
            return () => command.Validate();
        }

        public static Func<FeatureUpdatedEvent> WhenExtractingFeatureUpdatedEvent(this UpdateFeatureCommand command, ISystemClock clock)
        {
            return () => command.ExtractFeatureUpdatedEvent(clock);
        }

        public static Func<PathRemovedEvent> WhenExtractingPathRemovedEvent(this UpdateFeatureCommand command, ISystemClock clock)
        {
            return () => command.ExtractPathRemovedEvent(clock);
        }

        public static Func<PathCreatedEvent> WhenExtractingPathCreatedEvent(this UpdateFeatureCommand command, ISystemClock clock)
        {
            return () => command.ExtractPathCreatedEvent(clock);
        }

        public static void ThenWeGetAPathCreatedEvent(this Func<PathCreatedEvent> pathFunc, UpdateFeatureCommand command, ISystemClock clock)
        {
            var paths = pathFunc();

            paths.CreatedBy.Should().Be(command.UpdatedBy);
            paths.CreatedOn.Should().Be(clock.UtcNow);
            paths.FeatureAdded.Should().Be(command.NewName);
            paths.Path.Should().Be(command.NewPath);
        }

        public static void ThenWeGetAPathRemovedEvent(this Func<PathRemovedEvent> pathFunc, UpdateFeatureCommand command, ISystemClock clock)
        {
            var paths = pathFunc();

            paths.RemovedBy.Should().Be(command.UpdatedBy);
            paths.RemovedOn.Should().Be(clock.UtcNow);
            paths.FeatureRemoved.Should().Be(command.Name);
        }
        
        public static void ThenWeGetAFeatureUpdatedEvent(this Func<FeatureUpdatedEvent> featFunc, UpdateFeatureCommand command, ISystemClock clock)
        {
            var feat = featFunc();

            feat.UpdatedBy.Should().Be(command.UpdatedBy);
            feat.UpdatedOn.Should().Be(clock.UtcNow);
            feat.Name.Should().Be(command.Name);
            feat.Path.Should().Be(command.Path);
        }
    }
}