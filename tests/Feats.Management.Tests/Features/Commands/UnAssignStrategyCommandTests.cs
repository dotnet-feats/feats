
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
    public class UnAssignStrategyCommandTests : TestBase
    {
        [Test]
        public void GivenACommandWithAllSettings_WhenValidating_ThenNoExceptionIsThrown()
        {
            this.GivenValidCommand()
                .WhenValidating()
                .ThenNoExceptionIsThrown();
        }

        [Test]
        public void GivenACommandIsNull_WhenValidating_ThenArgumentNullIsThrown()
        {
            PublishFeatureCommand command = null;

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
        public void GivenACommandWithMissingStrategyName_WhenValidating_ThenArgumentNullIsThrown()
        {
            var command = this
                .GivenValidCommand();
            command.StrategyName = string.Empty;

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
        public void GivenACommandWithMissingUnassignedBy_WhenValidating_ThenArgumentNullIsThrown()
        {
            var command = this
                .GivenValidCommand();
            command.UnassignedBy = string.Empty;

            command
                .WhenValidating()
                .ThenExceptionIsThrown<ArgumentValidationException>();
        }
        
        [Test]
        public void GivenACommand_WhenExtractingEvent_ThenNoExceptionIsThrownt()
        {
            var clock = this.GivenClock();
            
            this.GivenValidCommand()
                .WhenExtractingEvent(clock)
                .ThenNoExceptionIsThrown();
        }
        
        [Test]
        public void GivenACommand_WhenExtractingEvent_ThenWeGetAPathCreatedEvent()
        {
            var clock = this.GivenClock();
            var request = this
                .GivenValidCommand();
            
            request
                .WhenExtractingEvent(clock)
                .ThenWeGetAFeaturePublishedEvent(request, clock);
        }
    }

    public static class UnassignStrategyCommandTestsExtensions 
    {
        public static UnAssignStrategyCommand GivenValidCommand(this UnAssignStrategyCommandTests tests)
        {
            return new UnAssignStrategyCommand
            {
                UnassignedBy = "🦄",
                Name = "bob ross 🎨🖌🖼",
                Path = "painting/in/winter",
                StrategyName = "patate",
            };
        }

        public static Action WhenValidating(this UnAssignStrategyCommand command)
        {
            return () => command.Validate();
        }

        public static Func<StrategyUnAssignedEvent> WhenExtractingEvent(
            this UnAssignStrategyCommand command,
            ISystemClock clock)
        {
            return () => command.ExtractStrategyUnassignedEvent(clock);
        }
        
        public static void ThenWeGetAFeaturePublishedEvent(
            this Func<StrategyUnAssignedEvent> featFunc,
             UnAssignStrategyCommand command, 
             ISystemClock clock)
        {
            var feat = featFunc();

            feat.UnassignedBy.Should().Be(command.UnassignedBy);
            feat.UnassignedOn.Should().Be(clock.UtcNow);
            feat.Name.Should().Be(command.Name);
            feat.Path.Should().Be(command.Path);
            feat.StrategyName.Should().Be(command.StrategyName);
        }
    }
}