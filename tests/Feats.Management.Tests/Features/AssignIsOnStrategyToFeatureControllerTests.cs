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
    public class AssignIsOnStrategyToFeatureControllerTests : TestBase
    {
        [Test]
        public async Task GivenRequest_WhenProcessingTheCommandIsSuccessful_ThenWeReturnOk()
        {
            var request = this
                .GivenRequest();
            var handler = this
                .GivenCommandHandler<AssignIsOnStrategyToFeatureCommand>()
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
                .GivenCommandHandler<AssignIsOnStrategyToFeatureCommand>()
                .WithException<AssignIsOnStrategyToFeatureCommand, TestException>();

            await this
                .GivenController(handler.Object)
                .WhenProcessingCommand(request)
                .ThenExceptionIsThrown<TestException, IActionResult>();
        }
    }

    public static class AssignIsOnStrategToFeatureControllerTestsExtensions
    {
        public static AssignIsOnStrategyToFeatureRequest GivenRequest(
            this AssignIsOnStrategyToFeatureControllerTests tests)
        {
            return new AssignIsOnStrategyToFeatureRequest
            {
                AssignedBy = "bob",
                Name = "Ross",
                Path = "🦄.🖼",
            };
        }

        public static AssignIsOnStrategyToFeatureController GivenController(
            this AssignIsOnStrategyToFeatureControllerTests tests, 
            IHandleCommand<AssignIsOnStrategyToFeatureCommand> handler)
        {
            return new AssignIsOnStrategyToFeatureController(handler);
        }

        public static Func<Task<IActionResult>> WhenProcessingCommand(
            this AssignIsOnStrategyToFeatureController controller,
            AssignIsOnStrategyToFeatureRequest request)
        {
            return () => controller.Post(request);
        }

        public static async Task ThenWeReturnOK(this Func<Task<IActionResult>> processingFunc)
        {
            var asyncResults = await processingFunc() as StatusCodeResult;
            asyncResults.StatusCode.Should().Be(200);
        }
    }
}