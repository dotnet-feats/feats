
using System;
using System.Linq;
using Feats.Common.Tests;
using Feats.Domain;
using Feats.Management.Features;
using Feats.Management.Features.Commands;
using FluentAssertions;
using NUnit.Framework;

public class UpdateFeatureRequestTests : TestBase
    {
        [Test]
        public void GivenARequestWithAllSettings_WhenValidating_ThenNoExceptionIsThrown()
        {
            this
                .GivenValidRequest()
                .WhenValidating()
                .ThenNoExceptionIsThrown();
        }

        [Test]
        public void GivenARequestIsNull_WhenValidating_ThenArgumentNullIsThrown()
        {
            UpdateFeatureRequest request = null;

            request
                .WhenValidating()
                .ThenExceptionIsThrown<ArgumentValidationException>();
        }

        [Test]
        public void GivenARequestWithMissingName_WhenValidating_ThenArgumentNullIsThrown()
        {
            var request = this
                .GivenValidRequest();
            request.Name = string.Empty;

            request
                .WhenValidating()
                .ThenExceptionIsThrown<ArgumentValidationException>();
        }

        [Test]
        public void GivenARequestWithMissingNewName_WhenValidating_ThenArgumentNullIsThrown()
        {
            var request = this
                .GivenValidRequest();
            request.NewName = string.Empty;

            request
                .WhenValidating()
                .ThenExceptionIsThrown<ArgumentValidationException>();
        }

        [Test]
        public void GivenARequestWithMissingPath_WhenValidating_ThenNoExceptionIsThrown()
        {
            var request = this
                .GivenValidRequest();
            request.Path = string.Empty;

            request
                .WhenValidating()
                .ThenNoExceptionIsThrown();
        }

        [Test]
        public void GivenARequestWithMissingNewPath_WhenValidating_ThenNoExceptionIsThrown()
        {
            var request = this
                .GivenValidRequest();
            request.NewPath = string.Empty;

            request
                .WhenValidating()
                .ThenNoExceptionIsThrown();
        }

        [Test]
        public void GivenARequestWithMissingUpdatedBy_WhenValidating_ThenArgumentNullIsThrown()
        {
            var request = this
                .GivenValidRequest();
            request.UpdatedBy = string.Empty;

            request
                .WhenValidating()
                .ThenExceptionIsThrown<ArgumentValidationException>();
        }
                
        [Test]
        public void GivenARequest_WhenExtractingCommand_ThenNoExceptoinIsThrown()
        {
            this.GivenValidRequest()
                .WhenExtractingCommand()
                .ThenNoExceptionIsThrown();
        }        
                        
        [Test]
        public void GivenARequest_WhenExtractingCommand_ThenCommandIsFilled()
        {
            var request = this
                .GivenValidRequest();
            request
                .WhenExtractingCommand()
                .ThenCommandIsFilled(request);
        }     
    }

    public static class UpdateFeatureRequestTestsExtensions 
    {
        public static UpdateFeatureRequest GivenValidRequest(this UpdateFeatureRequestTests tests)
        {
            return new UpdateFeatureRequest
            {
                UpdatedBy = "🦄",
                Name = "bob ross 🎨🖌🖼",
                Path = "painting/in/winter",
                NewName = "🎨🖌🖼",
                NewPath = "🌲",
            };
        }

        public static Action WhenValidating(this UpdateFeatureRequest request)
        {
            return () => request.Validate();
        }

        public static Func<UpdateFeatureCommand> WhenExtractingCommand(this UpdateFeatureRequest request)
        {
            return () => request.ToUpdateFeatureCommand();
        }

        public static void ThenCommandIsFilled(this Func<UpdateFeatureCommand> createFunc, UpdateFeatureRequest request)
        {
            var command = createFunc();

            command.Name.Should().Be(request.Name);   
            command.Path.Should().BeEquivalentTo(request.Path);
            command.NewName.Should().Be(request.NewName);   
            command.NewPath.Should().BeEquivalentTo(request.NewPath);
            command.UpdatedBy.Should().BeEquivalentTo(request.UpdatedBy);
        }
    }