using System;
using System.Threading.Tasks;
using Feats.Common.Tests;
using Feats.CQRS.Commands;
using Feats.Management.Features;
using Feats.Management.Features.Commands;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace Feats.Management.Tests.Features
{
    public class UnassignStrategyControllerTests : TestBase
    {
        [Test]
        public async Task GivenRequest_WhenProcessingTheCommandIsSuccessful_ThenWeReturnOk()
        {
            var request = this
                .GivenRequest();
            var handler = this
                .GivenCommandHandler<UnassignStrategyCommand>()
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
                .GivenCommandHandler<UnassignStrategyCommand>()
                .WithException<UnassignStrategyCommand, TestException>();

            await this
                .GivenController(handler.Object)
                .WhenProcessingCommand(request)
                .ThenExceptionIsThrown<TestException, IActionResult>();
        }
    }

    public static class UnassignStrategyControllerTestsExtensions
    {
        public static UnassignStrategyRequest GivenRequest(
            this UnassignStrategyControllerTests tests)
        {
            return new UnassignStrategyRequest
            {
                UnassignedBy = "bob",
                Name = "Ross",
                Path = "🦄.🖼",
                StrategyName = "patate",
            };
        }

        public static UnassignStrategyController GivenController(
            this UnassignStrategyControllerTests tests, 
            IHandleCommand<UnassignStrategyCommand> handler)
        {
            return new UnassignStrategyController(handler);
        }

        public static Func<Task<IActionResult>> WhenProcessingCommand(
            this UnassignStrategyController controller,
            UnassignStrategyRequest request)
        {
            return () => controller.Post(request);
        }
    }
}