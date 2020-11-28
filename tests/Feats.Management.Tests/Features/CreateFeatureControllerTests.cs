using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Feats.Common.Tests;
using Feats.CQRS.Commands;
using Feats.Management.Features;
using Feats.Management.Features.Commands;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace Feats.Management.Tests.Features
{
    public class CreateFeatureControllerTests : TestBase
    {
        [Test]
        public async Task GivenRequest_WhenProcessingTheCommandIsSuccessful_ThenWeReturnCreated()
        {
            var request = this
                .GivenRequest();
            var handler = this
                .GivenCommandHandler<CreateFeatureCommand>()
                .WithHandling();

            await this
                .GivenController(handler.Object)
                .WhenProcessingCommand(request)
                .ThenWeReturnCreated();
        }
        
        [Test]
        public async Task GivenRequest_WhenProcessingTheCommandItThrows_ThenThrow()
        {
            var request = this
                .GivenRequest();
            var handler = this
                .GivenCommandHandler<CreateFeatureCommand>()
                .WithException<CreateFeatureCommand, TestException>();

            await this
                .GivenController(handler.Object)
                .WhenProcessingCommand(request)
                .ThenExceptionIsThrown<TestException, IActionResult>();
        }
    }

    public static class CreateFeatureControllerTestsExtensions
    {
        public static CreateFeatureRequest GivenRequest(this CreateFeatureControllerTests tests)
        {
            return new CreateFeatureRequest
            {
                CreatedBy = "bob",
                Name = "Ross",
                Path = "🦄.🖼",
                StrategyNames = new List<string> { "little brush strokes" },
            };
        }

        public static CreateFeatureController GivenController(
            this CreateFeatureControllerTests tests, 
            IHandleCommand<CreateFeatureCommand> handler)
        {
            return new CreateFeatureController(handler);
        }

        public static Func<Task<IActionResult>> WhenProcessingCommand(
            this CreateFeatureController controller,
            CreateFeatureRequest request)
        {
            return () => controller.Put(request);
        }

        public static async Task ThenWeReturnCreated(this Func<Task<IActionResult>> processingFunc)
        {
            var asyncResults = await processingFunc() as StatusCodeResult;
            asyncResults.StatusCode.Should().Be(201);
        }
    }
}