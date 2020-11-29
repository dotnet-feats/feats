
using System;
using System.Text.Json;
using Feats.Common.Tests;
using Feats.Domain.Events;
using Feats.Domain.Strategies;
using Feats.Management.Features.Commands;
using FluentAssertions;
using Microsoft.Extensions.Internal;
using NUnit.Framework;

namespace Feats.Management.Tests.Features.Commands
{
    public class AssignIsOnStrategyToFeatureCommandTests : TestBase
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
                .ThenExceptionIsThrown<ArgumentNullException>();
        }

        [Test]
        public void GivenACommandWithMissingName_WhenValidating_ThenArgumentNullIsThrown()
        {
            var command = this
                .GivenValidCommand();
            command.Name = string.Empty;

            command
                .WhenValidating()
                .ThenExceptionIsThrown<ArgumentNullException>();
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
        public void GivenACommandWithMissingAssignedBy_WhenValidating_ThenArgumentNullIsThrown()
        {
            var command = this
                .GivenValidCommand();
            command.AssignedBy = string.Empty;

            command
                .WhenValidating()
                .ThenExceptionIsThrown<ArgumentNullException>();
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

    public static class AssignIsOnStrategyToFeatureCommandTestsExtensions 
    {
        public static AssignIsOnStrategyToFeatureCommand GivenValidCommand(this AssignIsOnStrategyToFeatureCommandTests tests)
        {
            return new AssignIsOnStrategyToFeatureCommand
            {
                AssignedBy = "🦄",
                Name = "bob ross 🎨🖌🖼",
                Path = "painting/in/winter",
                IsEnabled = true,
            };
        }

        public static Action WhenValidating(this AssignIsOnStrategyToFeatureCommand command)
        {
            return () => command.Validate();
        }

        public static Func<StrategyAssignedEvent> WhenExtractingEvent(this AssignIsOnStrategyToFeatureCommand command, ISystemClock clock)
        {
            return () => command.ExtractStrategyAssignedEvent(clock);
        }
        
        public static void ThenWeGetAFeaturePublishedEvent(this Func<StrategyAssignedEvent> featFunc, AssignIsOnStrategyToFeatureCommand command, ISystemClock clock)
        {
            var feat = featFunc();

            feat.AssignedBy.Should().Be(command.AssignedBy);
            feat.AssignedOn.Should().Be(clock.UtcNow);
            feat.Name.Should().Be(command.Name);
            feat.Path.Should().Be(command.Path);
            feat.StrategyName.Should().Be(StrategyNames.IsOn);
            feat.Settings.Should().Be(JsonSerializer.Serialize(new IsOnStrategySettings 
            {
                IsOn = command.IsEnabled,
            }));
        }
    }
}