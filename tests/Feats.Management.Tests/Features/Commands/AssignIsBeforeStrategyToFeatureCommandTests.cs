
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
    public class AssignIsBeforeStrategyToFeatureCommandTests : TestBase
    {
        private readonly ISystemClock _clock;

        public AssignIsBeforeStrategyToFeatureCommandTests()
        {
            this._clock = this.GivenClock();
        }

        [Test]
        public void GivenACommandWithAllSettings_WhenValidating_ThenNoExceptionIsThrown()
        {
            this.GivenValidCommand(this._clock)
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
                .GivenValidCommand(this._clock);
            command.Name = string.Empty;

            command
                .WhenValidating()
                .ThenExceptionIsThrown<ArgumentValidationException>();
        }

        [Test]
        public void GivenACommandWithMissingPath_WhenValidating_ThenNoExceptionIsThrown()
        {
            var command = this
                .GivenValidCommand(this._clock);
            command.Path = string.Empty;

            command
                .WhenValidating()
                .ThenNoExceptionIsThrown();
        }

        [Test]
        public void GivenACommandWithMinDateValue_WhenValidating_ThenArgumentNullIsThrown()
        {
            var command = this
                .GivenValidCommand(this._clock);
            command.Value = DateTimeOffset.MinValue;

            command
                .WhenValidating()
                .ThenExceptionIsThrown<ArgumentValidationException>();
        }
        
        [Test]
        public void GivenACommandWithMissingAssignedBy_WhenValidating_ThenArgumentNullIsThrown()
        {
            var command = this
                .GivenValidCommand(this._clock);
            command.AssignedBy = string.Empty;

            command
                .WhenValidating()
                .ThenExceptionIsThrown<ArgumentValidationException>();
        }
        
        [Test]
        public void GivenACommand_WhenExtractingEvent_ThenNoExceptionIsThrownt()
        {
            this.GivenValidCommand(this._clock)
                .WhenExtractingEvent(this._clock, this.GivenIStrategySettingsSerializer())
                .ThenNoExceptionIsThrown();
        }
        
        [Test]
        public void GivenACommand_WhenExtractingEvent_ThenWeGetAPathCreatedEvent()
        {
            var request = this
                .GivenValidCommand(this._clock);
            
            request
                .WhenExtractingEvent(this._clock, this.GivenIStrategySettingsSerializer())
                .ThenWeGetAFeaturePublishedEvent(request, this._clock);
        }
    }

    public static class AssignIsBeforeStrategyToFeatureCommandTestsExtensions 
    {
        public static AssignIsBeforeStrategyToFeatureCommand GivenValidCommand(
            this AssignIsBeforeStrategyToFeatureCommandTests tests,
            ISystemClock clock)
        {
            return new AssignIsBeforeStrategyToFeatureCommand
            {
                AssignedBy = "🦄",
                Name = "bob ross 🎨🖌🖼",
                Path = "painting/in/winter",
                Value = clock.UtcNow
            };
        }

        public static Action WhenValidating(this AssignIsBeforeStrategyToFeatureCommand command)
        {
            return () => command.Validate();
        }

        public static Func<StrategyAssignedEvent> WhenExtractingEvent(
            this AssignIsBeforeStrategyToFeatureCommand command,
            ISystemClock clock,
            IStrategySettingsSerializer serializer)
        {
            return () => command.ExtractStrategyAssignedEvent(
                clock,
                serializer);
        }
        
        public static void ThenWeGetAFeaturePublishedEvent(
            this Func<StrategyAssignedEvent> featFunc, 
            AssignIsBeforeStrategyToFeatureCommand command, 
            ISystemClock clock)
        {
            var feat = featFunc();

            feat.AssignedBy.Should().Be(command.AssignedBy);
            feat.AssignedOn.Should().Be(clock.UtcNow);
            feat.Name.Should().Be(command.Name);
            feat.Path.Should().Be(command.Path);
            feat.StrategyName.Should().Be(StrategyNames.IsBefore);
            feat.Settings.Should().Be(JsonSerializer.Serialize(new DateTimeOffsetStrategySettings 
            {
                Value = clock.UtcNow
            }));
        }
    }
}