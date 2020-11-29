
using System;
using System.Collections.Generic;
using System.Linq;
using Feats.Common.Tests;
using Feats.Domain.Events;
using Feats.Management.Features.Commands;
using Feats.Management.Features.Events;
using FluentAssertions;
using Microsoft.Extensions.Internal;
using NUnit.Framework;

namespace Feats.Management.Tests.Features.Commands
{
    public class CreateFeatureCommandTests : TestBase
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
            CreateFeatureCommand command = null;

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
        public void GivenACommandWithMissingCreatedBy_WhenValidating_ThenArgumentNullIsThrown()
        {
            var command = this
                .GivenValidCommand();
            command.CreatedBy = string.Empty;

            command
                .WhenValidating()
                .ThenExceptionIsThrown<ArgumentNullException>();
        }
        
        [Test]
        public void GivenACommand_WhenExtractingFeatureCreatedEvent_ThenNoExceptionIsThrownt()
        {
            var clock = this.GivenClock();
            
            this.GivenValidCommand()
                .WhenExtractingFeatureCreatedEvent(clock)
                .ThenNoExceptionIsThrown();
        }
        
        [Test]
        public void GivenACommand_WhenExtractingFeatureCreatedEvent_ThenWeGetAPathCreatedEvent()
        {
            var clock = this.GivenClock();
            var request = this
                .GivenValidCommand();
            
            request
                .WhenExtractingFeatureCreatedEvent(clock)
                .ThenWeGetAFeatureCreatedEvent(request, clock);
        } 
        
        [Test]
        public void GivenACommand_WhenExtractingPathCreatedEvent_ThenNoExceptionIsThrown()
        {
            var clock = this.GivenClock();

            this.GivenValidCommand()
                .WhenExtractingPathCreatedEvent(clock)
                .ThenNoExceptionIsThrown();
        }
        
        [Test]
        public void GivenACommand_WhenExtractingPathCreatedEvent_ThenWeGetAPathCreatedEvent()
        {
            var clock = this.GivenClock();
            var request = this
                .GivenValidCommand();
            
            request
                .WhenExtractingPathCreatedEvent(clock)
                .ThenWeGetAPathCreatedEvent(request, clock);
        }      
    }

    public static class CreateFeatureCommandTestsExtensions 
    {
        public static CreateFeatureCommand GivenValidCommand(this CreateFeatureCommandTests tests)
        {
            return new CreateFeatureCommand
            {
                CreatedBy = "🦄",
                Name = "bob ross 🎨🖌🖼",
                Path = "painting/in/winter",
            };
        }

        public static Action WhenValidating(this CreateFeatureCommand command)
        {
            return () => command.Validate();
        }

        public static Func<FeatureCreatedEvent> WhenExtractingFeatureCreatedEvent(this CreateFeatureCommand command, ISystemClock clock)
        {
            return () => command.ExtractFeatureCreatedEvent(clock);
        }

        public static Func<PathCreatedEvent> WhenExtractingPathCreatedEvent(this CreateFeatureCommand command, ISystemClock clock)
        {
            return () => command.ExtractPathCreatedEvent(clock);
        }

        public static void ThenWeGetAPathCreatedEvent(this Func<PathCreatedEvent> pathFunc, CreateFeatureCommand command, ISystemClock clock)
        {
            var paths = pathFunc();

            paths.CreatedBy.Should().Be(command.CreatedBy);
            paths.CreatedOn.Should().Be(clock.UtcNow);
            paths.FeatureAdded.Should().Be(command.Name);
        }
        
        public static void ThenWeGetAFeatureCreatedEvent(this Func<FeatureCreatedEvent> featFunc, CreateFeatureCommand command, ISystemClock clock)
        {
            var feat = featFunc();

            feat.CreatedBy.Should().Be(command.CreatedBy);
            feat.CreatedOn.Should().Be(clock.UtcNow);
            feat.Name.Should().Be(command.Name);
            feat.Path.Should().Be(command.Path);
        }
    }
}