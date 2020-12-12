using System;
using System.Net;
using System.Threading.Tasks;
using Feats.Common.Tests;
using Feats.Common.Tests.Commands;
using Feats.CQRS.Commands;
using Feats.Management.Features;
using Feats.Management.Features.Commands;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace Feats.Management.Tests.Features
{
    public class UpdateFeatureControllerTests : TestBase
    {
        [Test]
        public async Task GivenRequest_WhenProcessingTheCommandIsSuccessful_ThenWeReturnOK()
        {
            var request = this
                .GivenRequest();
            var handler = this
                .GivenCommandHandler<UpdateFeatureCommand>()
                .WithHandling();

            await this
                .GivenController(handler.Object)
                .WhenProcessingCommand(request)
                .ThenWeReturnOK();
        }
        
        [Test]
        public async Task GivenRequestWithNothingChanging_WhenProcessingTheCommandIsSuccessful_ThenWeReturnNoContent()
        {
            var request = this
                .GivenRequest();
            request.NewName = request.Name;
            request.NewPath = request.Path;

            var handler = this
                .GivenCommandHandler<UpdateFeatureCommand>()
                .WithHandling();

            await this
                .GivenController(handler.Object)
                .WhenProcessingCommand(request)
                .ThenWeReturnNoContent();
        }

        [Test]
        public async Task GivenRequest_WhenProcessingTheCommandItThrows_ThenThrow()
        {
            var request = this
                .GivenRequest();
            var handler = this
                .GivenCommandHandler<UpdateFeatureCommand>()
                .WithException<UpdateFeatureCommand, TestException>();

            await this
                .GivenController(handler.Object)
                .WhenProcessingCommand(request)
                .ThenExceptionIsThrown<TestException, IActionResult>();
        }
    }

    public static class UpdateFeatureControllerTestsExtensions
    {
        public static UpdateFeatureRequest GivenRequest(this UpdateFeatureControllerTests tests)
        {
            return new UpdateFeatureRequest
            {
                UpdatedBy = "bob",
                Name = "Ross",
                Path = "🦄.🖼",
                NewName = "djp0n3",
                NewPath = "🎶",
            };
        }

        public static UpdateFeatureController GivenController(
            this UpdateFeatureControllerTests tests, 
            IHandleCommand<UpdateFeatureCommand> handler)
        {
            return new UpdateFeatureController(handler);
        }

        public static Func<Task<IActionResult>> WhenProcessingCommand(
            this UpdateFeatureController controller,
            UpdateFeatureRequest request)
        {
            return () => controller.Post(request);
        }
        
        public static async Task ThenWeReturnNoContent(this Func<Task<IActionResult>> processingFunc)
        {
            var asyncResults = await processingFunc() as StatusCodeResult;
            asyncResults.StatusCode.Should().Be((int)HttpStatusCode.NoContent);
        }
    }
}