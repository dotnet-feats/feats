
using System;
using System.Collections.Generic;
using System.Text.Json;
using Feats.Common.Tests;
using Feats.Common.Tests.Strategies;
using Feats.Common.Tests.Strategies.TestExtensions;
using Feats.Domain;
using Feats.Domain.Events;
using Feats.Domain.Exceptions;
using Feats.Domain.Strategies;
using Feats.Management.Features.Commands;
using FluentAssertions;
using Microsoft.Extensions.Internal;
using NUnit.Framework;

namespace Feats.Management.Tests.Features.Commands
{
    public class AssignIsLowerThanStrategyToFeatureCommandTests : TestBase
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
                .ThenExceptionIsThrown<ArgumentValidationException>();
        }
        
        [Test]
        public void GivenACommand_WhenExtractingEvent_ThenNoExceptionIsThrownt()
        {
            var clock = this.GivenClock();
            
            this.GivenValidCommand()
                .WhenExtractingEvent(clock, this.GivenIStrategySettingsSerializer())
                .ThenNoExceptionIsThrown();
        }
        
        [Test]
        public void GivenACommand_WhenExtractingEvent_ThenWeGetAPathCreatedEvent()
        {
            var clock = this.GivenClock();
            var request = this
                .GivenValidCommand();
            
            request
                .WhenExtractingEvent(clock, this.GivenIStrategySettingsSerializer())
                .ThenWeGetAFeaturePublishedEvent(request, clock);
        }
    }

    public static class AssignIsLowerThanStrategyToFeatureCommandTestsExtensions 
    {
        public static AssignIsLowerThanStrategyToFeatureCommand GivenValidCommand(this AssignIsLowerThanStrategyToFeatureCommandTests tests)
        {
            return new AssignIsLowerThanStrategyToFeatureCommand
            {
                AssignedBy = "🦄",
                Name = "bob ross 🎨🖌🖼",
                Path = "painting/in/winter",
                Value = 8.88545
            };
        }

        public static Action WhenValidating(this AssignIsLowerThanStrategyToFeatureCommand command)
        {
            return () => command.Validate();
        }

        public static Func<StrategyAssignedEvent> WhenExtractingEvent(
            this AssignIsLowerThanStrategyToFeatureCommand command,
            ISystemClock clock,
            IStrategySettingsSerializer serializer)
        {
            return () => command.ExtractStrategyAssignedEvent(
                clock,
                serializer);
        }
        
        public static void ThenWeGetAFeaturePublishedEvent(this Func<StrategyAssignedEvent> featFunc, AssignIsLowerThanStrategyToFeatureCommand command, ISystemClock clock)
        {
            var feat = featFunc();

            feat.AssignedBy.Should().Be(command.AssignedBy);
            feat.AssignedOn.Should().Be(clock.UtcNow);
            feat.Name.Should().Be(command.Name);
            feat.Path.Should().Be(command.Path);
            feat.StrategyName.Should().Be(StrategyNames.IsLowerThan);
            feat.Settings.Should().Be(JsonSerializer.Serialize(new NumericalStrategySettings() 
            {
                Value = command.Value
            }));
        }
    }
}