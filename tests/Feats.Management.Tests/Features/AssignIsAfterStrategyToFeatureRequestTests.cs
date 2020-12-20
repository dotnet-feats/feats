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
    public class AssignIsAfterStrategyToFeatureRequestTests : TestBase
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
            AssignIsAfterStrategyToFeatureRequest request = null;

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
        public void GivenARequestWithNullDateValue_WhenValidating_ThenExceptionIsThrown()
        {
            var request = this
                .GivenValidRequest();
            request.Value = null;

            request
                .WhenValidating()
                .ThenExceptionIsThrown<ArgumentValidationException>();
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
    }

    public static class AssignIsAfterToFeatureRequestTestsExtensions 
    {
        public static AssignIsAfterStrategyToFeatureRequest GivenValidRequest(this AssignIsAfterStrategyToFeatureRequestTests tests)
        {
            return new AssignIsAfterStrategyToFeatureRequest
            {
                AssignedBy = "🦄",
                Name = "bob ross 🎨🖌🖼",
                Path = "painting/in/winter",
                Value = DateTimeOffset.Now
            };
        }

        public static Action WhenValidating(this AssignIsAfterStrategyToFeatureRequest request)
        {
            return () => request.Validate();
        }

        public static Func<AssignIsAfterStrategyToFeatureCommand> WhenExtractingCommand(this AssignIsAfterStrategyToFeatureRequest request)
        {
            return () => request.ToAssignIsAfterStrategyToFeatureCommand();
        }

        public static void ThenCommandIsFilled(this Func<AssignIsAfterStrategyToFeatureCommand> createFunc, AssignIsAfterStrategyToFeatureRequest request)
        {
            var command = createFunc();

            command.Name.Should().Be(request.Name);   
            command.Path.Should().Be(request.Path);
            command.AssignedBy.Should().Be(request.AssignedBy);
            command.Value.Should().Be(request.Value.GetValueOrDefault());
        }
    }
}