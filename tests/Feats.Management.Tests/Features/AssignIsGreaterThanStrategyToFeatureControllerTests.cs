using System;
using System.Collections.Generic;
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
    public class AssignIsGreaterThanStrategyToFeatureControllerTests : TestBase
    {
        [Test]
        public async Task GivenRequest_WhenProcessingTheCommandIsSuccessful_ThenWeReturnOk()
        {
            var request = this
                .GivenRequest();
            var handler = this
                .GivenCommandHandler<AssignIsGreaterThanStrategyToFeatureCommand>()
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
                .GivenCommandHandler<AssignIsGreaterThanStrategyToFeatureCommand>()
                .WithException<AssignIsGreaterThanStrategyToFeatureCommand, TestException>();

            await this
                .GivenController(handler.Object)
                .WhenProcessingCommand(request)
                .ThenExceptionIsThrown<TestException, IActionResult>();
        }
    }

    public static class AssignIsGreaterThanStrategyToFeatureControllerTestsExtensions
    {
        public static AssignIsGreaterThanStrategyToFeatureRequest GivenRequest(
            this AssignIsGreaterThanStrategyToFeatureControllerTests tests)
        {
            return new AssignIsGreaterThanStrategyToFeatureRequest
            {
                AssignedBy = "bob",
                Name = "Ross",
                Path = "🦄.🖼",
                Value = -567.12313213131231313
            };
        }

        public static AssignIsGreaterThanStrategyToFeatureController GivenController(
            this AssignIsGreaterThanStrategyToFeatureControllerTests tests, 
            IHandleCommand<AssignIsGreaterThanStrategyToFeatureCommand> handler)
        {
            return new AssignIsGreaterThanStrategyToFeatureController(handler);
        }

        public static Func<Task<IActionResult>> WhenProcessingCommand(
            this AssignIsGreaterThanStrategyToFeatureController controller,
            AssignIsGreaterThanStrategyToFeatureRequest request)
        {
            return () => controller.Post(request);
        }
    }
}