
using System;
using System.Collections.Generic;
using System.Linq;
using Feats.Common.Tests;
using Feats.Management.Features;
using Feats.Management.Features.Commands;
using FluentAssertions;
using NUnit.Framework;

public class CreateFeatureRequestTests : TestBase
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
        public void GivenARequestWithMissingStrategies_WhenValidating_ThenNoExceptionIsThrown()
        {
            var command = this
                .GivenValidRequest();
            command.StrategyNames = Enumerable.Empty<string>();

            command
                .WhenValidating()
                .ThenNoExceptionIsThrown();
        }

        [Test]
        public void GivenARequestIsNUll_WhenValidating_ThenArgumentNullIsThrown()
        {
            CreateFeatureRequest request = null;

            request
                .WhenValidating()
                .ThenExceptionIsThrown<ArgumentNullException>();
        }

        [Test]
        public void GivenARequestWithMissingName_WhenValidating_ThenArgumentNullIsThrown()
        {
            var request = this
                .GivenValidRequest();
            request.Name = string.Empty;

            request
                .WhenValidating()
                .ThenExceptionIsThrown<ArgumentNullException>();
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
            request.CreatedBy = string.Empty;

            request
                .WhenValidating()
                .ThenExceptionIsThrown<ArgumentNullException>();
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

    public static class CreateFeatureRequestTestsExtensions 
    {
        public static CreateFeatureRequest GivenValidRequest(this CreateFeatureRequestTests tests)
        {
            return new CreateFeatureRequest
            {
                CreatedBy = "🦄",
                Name = "bob ross 🎨🖌🖼",
                Path = "painting/in/winter",
                StrategyNames = new List<string> { "one", "two" },
            };
        }

        public static Action WhenValidating(this CreateFeatureRequest request)
        {
            return () => request.Validate();
        }

        public static Func<CreateFeatureCommand> WhenExtractingCommand(this CreateFeatureRequest request)
        {
            return () => request.ToCreateFeatureCommand();
        }

        public static void ThenCommandIsFilled(this Func<CreateFeatureCommand> createFunc, CreateFeatureRequest request)
        {
            var command = createFunc();

            command.StrategyNames.Should().BeEquivalentTo(request.StrategyNames);   
            command.Name.Should().Be(request.Name);   
            command.Path.Should().BeEquivalentTo(request.Path);
            command.CreatedBy.Should().BeEquivalentTo(request.CreatedBy);
        }
    }