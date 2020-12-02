using System;
using System.Threading.Tasks;
using Feats.Common.Tests;
using Feats.CQRS.Commands;
using Feats.Management.Features;
using Feats.Management.Features.Commands;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace Feats.Management.Tests.Features
{
    public class PublishFeatureControllerTests : TestBase
    {
        [Test]
        public async Task GivenRequest_WhenProcessingTheCommandIsSuccessful_ThenWeReturnOk()
        {
            var request = this
                .GivenRequest();
            var handler = this
                .GivenCommandHandler<PublishFeatureCommand>()
                .WithHandling();

            await this
                .GivenController(handler.Object)
                .WhenProcessingCommand(request)
                .ThenWeReturnOK();
        }
        
        [Test]
        public async Task GivenRequest_WhenProcessingTheCommandItThrows_ThenThrow()
        {
            var request = this
                .GivenRequest();
            var handler = this
                .GivenCommandHandler<PublishFeatureCommand>()
                .WithException<PublishFeatureCommand, TestException>();

            await this
                .GivenController(handler.Object)
                .WhenProcessingCommand(request)
                .ThenExceptionIsThrown<TestException, IActionResult>();
        }
    }

    public static class PublishFeatureControllerTestsExtensions
    {
        public static PublishFeatureRequest GivenRequest(this PublishFeatureControllerTests tests)
        {
            return new PublishFeatureRequest
            {
                PublishedBy = "bob",
                Name = "Ross",
                Path = "🦄.🖼",
            };
        }

        public static PublishFeatureController GivenController(
            this PublishFeatureControllerTests tests, 
            IHandleCommand<PublishFeatureCommand> handler)
        {
            return new PublishFeatureController(handler);
        }

        public static Func<Task<IActionResult>> WhenProcessingCommand(
            this PublishFeatureController controller,
            PublishFeatureRequest request)
        {
            return () => controller.Post(request);
        }
    }
}