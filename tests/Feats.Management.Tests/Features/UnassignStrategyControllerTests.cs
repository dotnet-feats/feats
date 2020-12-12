using System;
using System.Threading.Tasks;
using Feats.Common.Tests;
using Feats.Common.Tests.Commands;
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
                .GivenCommandHandler<UnAssignStrategyCommand>()
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
                .GivenCommandHandler<UnAssignStrategyCommand>()
                .WithException<UnAssignStrategyCommand, TestException>();

            await this
                .GivenController(handler.Object)
                .WhenProcessingCommand(request)
                .ThenExceptionIsThrown<TestException, IActionResult>();
        }
    }

    public static class UnassignStrategyControllerTestsExtensions
    {
        public static UnAssignStrategyRequest GivenRequest(
            this UnassignStrategyControllerTests tests)
        {
            return new UnAssignStrategyRequest
            {
                UnassignedBy = "bob",
                Name = "Ross",
                Path = "🦄.🖼",
                StrategyName = "patate",
            };
        }

        public static UnAssignStrategyController GivenController(
            this UnassignStrategyControllerTests tests, 
            IHandleCommand<UnAssignStrategyCommand> handler)
        {
            return new UnAssignStrategyController(handler);
        }

        public static Func<Task<IActionResult>> WhenProcessingCommand(
            this UnAssignStrategyController controller,
            UnAssignStrategyRequest request)
        {
            return () => controller.Post(request);
        }
    }
}