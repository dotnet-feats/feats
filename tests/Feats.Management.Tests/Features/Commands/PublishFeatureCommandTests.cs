
using System;
using Feats.Common.Tests;
using Feats.Management.Features.Commands;
using Feats.Management.Features.Events;
using FluentAssertions;
using Microsoft.Extensions.Internal;
using NUnit.Framework;

namespace Feats.Management.Tests.Features.Commands
{
    public class PublishFeatureCommandTests : TestBase
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
        public void GivenACommandWithMissingCreatedBy_WhenValidating_ThenArgumentNullIsThrown()
        {
            var command = this
                .GivenValidCommand();
            command.PublishedBy = string.Empty;

            command
                .WhenValidating()
                .ThenExceptionIsThrown<ArgumentNullException>();
        }
        
        [Test]
        public void GivenACommand_WhenExtractingFeaturePublishedEvent_ThenNoExceptionIsThrownt()
        {
            var clock = this.GivenClock();
            
            this.GivenValidCommand()
                .WhenExtractingFeaturePublishedEvent(clock)
                .ThenNoExceptionIsThrown();
        }
        
        [Test]
        public void GivenACommand_WhenExtractingFeaturePublishedEvent_ThenWeGetAPathCreatedEvent()
        {
            var clock = this.GivenClock();
            var request = this
                .GivenValidCommand();
            
            request
                .WhenExtractingFeaturePublishedEvent(clock)
                .ThenWeGetAFeaturePublishedEvent(request, clock);
        }
    }

    public static class PublishFeatureCommandTestsExtensions 
    {
        public static PublishFeatureCommand GivenValidCommand(this PublishFeatureCommandTests tests)
        {
            return new PublishFeatureCommand
            {
                PublishedBy = "🦄",
                Name = "bob ross 🎨🖌🖼",
                Path = "painting/in/winter",
            };
        }

        public static Action WhenValidating(this PublishFeatureCommand command)
        {
            return () => command.Validate();
        }

        public static Func<FeaturePublishedEvent> WhenExtractingFeaturePublishedEvent(this PublishFeatureCommand command, ISystemClock clock)
        {
            return () => command.ExtractFeaturePublishedEvent(clock);
        }
        
        public static void ThenWeGetAFeaturePublishedEvent(this Func<FeaturePublishedEvent> featFunc, PublishFeatureCommand command, ISystemClock clock)
        {
            var feat = featFunc();

            feat.PublishedBy.Should().Be(command.PublishedBy);
            feat.PublishedOn.Should().Be(clock.UtcNow);
            feat.Name.Should().Be(command.Name);
            feat.Path.Should().Be(command.Path);
        }
    }
}