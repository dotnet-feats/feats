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
    public class AssignIsInListStrategyToFeatureControllerTests : TestBase
    {
        [Test]
        public async Task GivenRequest_WhenProcessingTheCommandIsSuccessful_ThenWeReturnOk()
        {
            var request = this
                .GivenRequest();
            var handler = this
                .GivenCommandHandler<AssignIsInListStrategyToFeatureCommand>()
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
                .GivenCommandHandler<AssignIsInListStrategyToFeatureCommand>()
                .WithException<AssignIsInListStrategyToFeatureCommand, TestException>();

            await this
                .GivenController(handler.Object)
                .WhenProcessingCommand(request)
                .ThenExceptionIsThrown<TestException, IActionResult>();
        }
    }

    public static class AssignIsInListStrategToFeatureControllerTestsExtensions
    {
        public static AssignIsInListStrategyToFeatureRequest GivenRequest(
            this AssignIsInListStrategyToFeatureControllerTests tests)
        {
            return new AssignIsInListStrategyToFeatureRequest
            {
                AssignedBy = "bob",
                Name = "Ross",
                Path = "🦄.🖼",
                Items = new List<string> { "😜" },
            };
        }

        public static AssignIsInListStrategyToFeatureController GivenController(
            this AssignIsInListStrategyToFeatureControllerTests tests, 
            IHandleCommand<AssignIsInListStrategyToFeatureCommand> handler)
        {
            return new AssignIsInListStrategyToFeatureController(handler);
        }

        public static Func<Task<IActionResult>> WhenProcessingCommand(
            this AssignIsInListStrategyToFeatureController controller,
            AssignIsInListStrategyToFeatureRequest request)
        {
            return () => controller.Post(request);
        }
    }
}