using System;
using System.Collections.Generic;
using System.Linq;
using Feats.Common.Tests;
using Feats.Domain;
using Feats.Domain.Exceptions;
using Feats.Management.Features;
using Feats.Management.Features.Commands;
using FluentAssertions;
using NUnit.Framework;

namespace Feats.Management.Tests.Features
{
    public class AssignIsInListStrategyToFeatureRequestTests : TestBase
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
            AssignIsInListStrategyToFeatureRequest request = null;

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
        public void GivenARequestWithMissingListName_WhenValidating_ThenNoExceptionIsThrown()
        {
            var request = this
                .GivenValidRequest();
            request.ListName = string.Empty;

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
            request.Items = Enumerable.Empty<string>();
            
            request
                .WhenExtractingCommand()
                .ThenCommandIsFilled(request);
        }     
    }

    public static class AssignIsInListToFeatureRequestTestsExtensions 
    {
        public static AssignIsInListStrategyToFeatureRequest GivenValidRequest(this AssignIsInListStrategyToFeatureRequestTests tests)
        {
            return new AssignIsInListStrategyToFeatureRequest
            {
                AssignedBy = "🦄",
                Name = "bob ross 🎨🖌🖼",
                Path = "painting/in/winter",
                Items = new List<string> { "are you suggesting coconuts migrate" },
                ListName =  "boom"
            };
        }

        public static Action WhenValidating(this AssignIsInListStrategyToFeatureRequest request)
        {
            return () => request.Validate();
        }

        public static Func<AssignIsInListStrategyToFeatureCommand> WhenExtractingCommand(this AssignIsInListStrategyToFeatureRequest request)
        {
            return () => request.ToAssignIsInListStrategyToFeatureCommand();
        }

        public static void ThenCommandIsFilled(this Func<AssignIsInListStrategyToFeatureCommand> createFunc, AssignIsInListStrategyToFeatureRequest request)
        {
            var command = createFunc();

            command.Name.Should().Be(request.Name);   
            command.Path.Should().Be(request.Path);
            command.ListName.Should().Be(request.ListName);
            command.AssignedBy.Should().Be(request.AssignedBy);
            command.Items.Should().BeEquivalentTo(request.Items);
        }
    }
}