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
    public class AssignIsAfterStrategyToFeatureControllerTests : TestBase
    {
        [Test]
        public async Task GivenRequest_WhenProcessingTheCommandIsSuccessful_ThenWeReturnOk()
        {
            var request = this
                .GivenRequest();
            var handler = this
                .GivenCommandHandler<AssignIsAfterStrategyToFeatureCommand>()
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
                .GivenCommandHandler<AssignIsAfterStrategyToFeatureCommand>()
                .WithException<AssignIsAfterStrategyToFeatureCommand, TestException>();

            await this
                .GivenController(handler.Object)
                .WhenProcessingCommand(request)
                .ThenExceptionIsThrown<TestException, IActionResult>();
        }
    }

    public static class AssignIsAfterStrategyToFeatureControllerTestsExtensions
    {
        public static AssignIsAfterStrategyToFeatureRequest GivenRequest(
            this AssignIsAfterStrategyToFeatureControllerTests tests)
        {
            return new AssignIsAfterStrategyToFeatureRequest
            {
                AssignedBy = "bob",
                Name = "Ross",
                Path = "🦄.🖼",
                Value = DateTimeOffset.Now
            };
        }

        public static AssignIsAfterStrategyToFeatureController GivenController(
            this AssignIsAfterStrategyToFeatureControllerTests tests, 
            IHandleCommand<AssignIsAfterStrategyToFeatureCommand> handler)
        {
            return new AssignIsAfterStrategyToFeatureController(handler);
        }

        public static Func<Task<IActionResult>> WhenProcessingCommand(
            this AssignIsAfterStrategyToFeatureController controller,
            AssignIsAfterStrategyToFeatureRequest request)
        {
            return () => controller.Post(request);
        }
    }
}