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
    public class AssignIsLowerThanStrategyToFeatureControllerTests : TestBase
    {
        [Test]
        public async Task GivenRequest_WhenProcessingTheCommandIsSuccessful_ThenWeReturnOk()
        {
            var request = this
                .GivenRequest();
            var handler = this
                .GivenCommandHandler<AssignIsLowerThanStrategyToFeatureCommand>()
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
                .GivenCommandHandler<AssignIsLowerThanStrategyToFeatureCommand>()
                .WithException<AssignIsLowerThanStrategyToFeatureCommand, TestException>();

            await this
                .GivenController(handler.Object)
                .WhenProcessingCommand(request)
                .ThenExceptionIsThrown<TestException, IActionResult>();
        }
    }

    public static class AssignIsLowerThanStrategyToFeatureControllerTestsExtensions
    {
        public static AssignIsLowerThanStrategyToFeatureRequest GivenRequest(
            this AssignIsLowerThanStrategyToFeatureControllerTests tests)
        {
            return new AssignIsLowerThanStrategyToFeatureRequest
            {
                AssignedBy = "bob",
                Name = "Ross",
                Path = "🦄.🖼",
                Value = -567.12313213131231313
            };
        }

        public static AssignIsLowerThanStrategyToFeatureController GivenController(
            this AssignIsLowerThanStrategyToFeatureControllerTests tests, 
            IHandleCommand<AssignIsLowerThanStrategyToFeatureCommand> handler)
        {
            return new AssignIsLowerThanStrategyToFeatureController(handler);
        }

        public static Func<Task<IActionResult>> WhenProcessingCommand(
            this AssignIsLowerThanStrategyToFeatureController controller,
            AssignIsLowerThanStrategyToFeatureRequest request)
        {
            return () => controller.Post(request);
        }
    }
}