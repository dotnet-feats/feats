
using System;
using System.Linq;
using Feats.Common.Tests;
using Feats.Domain;
using Feats.Management.Features;
using Feats.Management.Features.Commands;
using FluentAssertions;
using NUnit.Framework;

public class UnassignStrategyRequestTests : TestBase
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
            UnassignStrategyRequest request = null;

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
        public static UnassignStrategyRequest GivenValidRequest(this UnassignStrategyRequestTests tests)
        {
            return new UnassignStrategyRequest
            {
                UnassignedBy = "🦄",
                Name = "bob ross 🎨🖌🖼",
                Path = "painting/in/winter",
                StrategyName = "hello",
            };
        }

        public static Action WhenValidating(this UnassignStrategyRequest request)
        {
            return () => request.Validate();
        }

        public static Func<UnassignStrategyCommand> WhenExtractingCommand(this UnassignStrategyRequest request)
        {
            return () => request.ToUnassignStrategyCommand();
        }

        public static void ThenCommandIsFilled(this Func<UnassignStrategyCommand> createFunc, UnassignStrategyRequest request)
        {
            var command = createFunc();

            command.Name.Should().Be(request.Name);   
            command.Path.Should().BeEquivalentTo(request.Path);
            command.UnassignedBy.Should().BeEquivalentTo(request.UnassignedBy);
            command.StrategyName.Should().Be(request.StrategyName);
        }
    }