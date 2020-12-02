
using System;
using System.Linq;
using Feats.Common.Tests;
using Feats.Domain;
using Feats.Management.Features;
using Feats.Management.Features.Commands;
using FluentAssertions;
using NUnit.Framework;

public class PublishFeatureRequestTests : TestBase
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
        public void GivenARequestIsNUll_WhenValidating_ThenArgumentNullIsThrown()
        {
            PublishFeatureRequest request = null;

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
        public void GivenARequestWithMissingCreatedBy_WhenValidating_ThenArgumentNullIsThrown()
        {
            var request = this
                .GivenValidRequest();
            request.PublishedBy = string.Empty;

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

    public static class PublishFeatureRequestTestsExtensions 
    {
        public static PublishFeatureRequest GivenValidRequest(this PublishFeatureRequestTests tests)
        {
            return new PublishFeatureRequest
            {
                PublishedBy = "🦄",
                Name = "bob ross 🎨🖌🖼",
                Path = "painting/in/winter",
            };
        }

        public static Action WhenValidating(this PublishFeatureRequest request)
        {
            return () => request.Validate();
        }

        public static Func<PublishFeatureCommand> WhenExtractingCommand(this PublishFeatureRequest request)
        {
            return () => request.ToPublishFeatureCommand();
        }

        public static void ThenCommandIsFilled(this Func<PublishFeatureCommand> createFunc, PublishFeatureRequest request)
        {
            var command = createFunc();

            command.Name.Should().Be(request.Name);   
            command.Path.Should().BeEquivalentTo(request.Path);
            command.PublishedBy.Should().BeEquivalentTo(request.PublishedBy);
        }
    }