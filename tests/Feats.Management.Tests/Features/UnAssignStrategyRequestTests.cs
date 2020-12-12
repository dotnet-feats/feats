using System;
using Feats.Common.Tests;
using Feats.Domain;
using Feats.Domain.Exceptions;
using Feats.Management.Features;
using Feats.Management.Features.Commands;
using FluentAssertions;
using NUnit.Framework;

namespace Feats.Management.Tests.Features
{
    public class UnAssignStrategyRequestTests : TestBase
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
            UnAssignStrategyRequest request = null;

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
            request.UnassignedBy = string.Empty;

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
        public void GivenARequestWithMissingStrategy_WhenValidatingCommand_ThenWeThrow()
        {
            var request = this
                .GivenValidRequest();
            request.StrategyName = string.Empty;
            
            request
                .WhenValidating()
                .ThenExceptionIsThrown<ArgumentValidationException>();
        }     
    }

    public static class UnassignStrategyRequestTestsExtensions 
    {
        public static UnAssignStrategyRequest GivenValidRequest(this UnAssignStrategyRequestTests tests)
        {
            return new UnAssignStrategyRequest
            {
                UnassignedBy = "🦄",
                Name = "bob ross 🎨🖌🖼",
                Path = "painting/in/winter",
                StrategyName = "hello",
            };
        }

        public static Action WhenValidating(this UnAssignStrategyRequest request)
        {
            return () => request.Validate();
        }

        public static Func<UnAssignStrategyCommand> WhenExtractingCommand(this UnAssignStrategyRequest request)
        {
            return () => request.ToUnAssignStrategyCommand();
        }

        public static void ThenCommandIsFilled(this Func<UnAssignStrategyCommand> createFunc, UnAssignStrategyRequest request)
        {
            var command = createFunc();

            command.Name.Should().Be(request.Name);   
            command.Path.Should().BeEquivalentTo(request.Path);
            command.UnassignedBy.Should().BeEquivalentTo(request.UnassignedBy);
            command.StrategyName.Should().Be(request.StrategyName);
        }
    }
}