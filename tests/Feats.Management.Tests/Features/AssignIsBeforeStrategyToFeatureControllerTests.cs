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
    public class AssignIsBeforeStrategyToFeatureControllerTests : TestBase
    {
        [Test]
        public async Task GivenRequest_WhenProcessingTheCommandIsSuccessful_ThenWeReturnOk()
        {
            var request = this
                .GivenRequest();
            var handler = this
                .GivenCommandHandler<AssignIsBeforeStrategyToFeatureCommand>()
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
                .GivenCommandHandler<AssignIsBeforeStrategyToFeatureCommand>()
                .WithException<AssignIsBeforeStrategyToFeatureCommand, TestException>();

            await this
                .GivenController(handler.Object)
                .WhenProcessingCommand(request)
                .ThenExceptionIsThrown<TestException, IActionResult>();
        }
    }

    public static class AssignIsBeforeStrategyToFeatureControllerTestsExtensions
    {
        public static AssignIsBeforeStrategyToFeatureRequest GivenRequest(
            this AssignIsBeforeStrategyToFeatureControllerTests tests)
        {
            return new AssignIsBeforeStrategyToFeatureRequest
            {
                AssignedBy = "bob",
                Name = "Ross",
                Path = "🦄.🖼",
                Value = DateTimeOffset.Now
            };
        }

        public static AssignIsBeforeStrategyToFeatureController GivenController(
            this AssignIsBeforeStrategyToFeatureControllerTests tests, 
            IHandleCommand<AssignIsBeforeStrategyToFeatureCommand> handler)
        {
            return new AssignIsBeforeStrategyToFeatureController(handler);
        }

        public static Func<Task<IActionResult>> WhenProcessingCommand(
            this AssignIsBeforeStrategyToFeatureController controller,
            AssignIsBeforeStrategyToFeatureRequest request)
        {
            return () => controller.Post(request);
        }
    }
}