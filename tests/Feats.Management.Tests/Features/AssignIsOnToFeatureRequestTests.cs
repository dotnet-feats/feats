
using System;
using System.Linq;
using Feats.Common.Tests;
using Feats.Domain;
using Feats.Management.Features;
using Feats.Management.Features.Commands;
using FluentAssertions;
using NUnit.Framework;

public class AssignIsOnToFeatureRequestTests : TestBase
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
            AssignIsOnStrategyToFeatureRequest request = null;

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
            request.AssignedBy = string.Empty;

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
                        
        [Test]
        public void GivenARequestWithDisabledStrategy_WhenExtractingCommand_ThenCommandIsFilled()
        {
            var request = this
                .GivenValidRequest();
            request.IsOn = false;
            
            request
                .WhenExtractingCommand()
                .ThenCommandIsFilled(request);
        }     
    }

    public static class AssignIsOnToFeatureRequestTestsExtensions 
    {
        public static AssignIsOnStrategyToFeatureRequest GivenValidRequest(this AssignIsOnToFeatureRequestTests tests)
        {
            return new AssignIsOnStrategyToFeatureRequest
            {
                AssignedBy = "🦄",
                Name = "bob ross 🎨🖌🖼",
                Path = "painting/in/winter",
            };
        }

        public static Action WhenValidating(this AssignIsOnStrategyToFeatureRequest request)
        {
            return () => request.Validate();
        }

        public static Func<AssignIsOnStrategyToFeatureCommand> WhenExtractingCommand(this AssignIsOnStrategyToFeatureRequest request)
        {
            return () => request.ToAssignIsOnStrategyToFeatureCommand();
        }

        public static void ThenCommandIsFilled(this Func<AssignIsOnStrategyToFeatureCommand> createFunc, AssignIsOnStrategyToFeatureRequest request)
        {
            var command = createFunc();

            command.Name.Should().Be(request.Name);   
            command.Path.Should().BeEquivalentTo(request.Path);
            command.AssignedBy.Should().BeEquivalentTo(request.AssignedBy);
            command.IsEnabled.Should().Be(request.IsOn);
        }
    }