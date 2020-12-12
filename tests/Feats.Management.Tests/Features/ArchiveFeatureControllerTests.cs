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
    public class ArchiveFeatureControllerTests : TestBase
    {
        [Test]
        public async Task GivenRequest_WhenProcessingTheCommandIsSuccessful_ThenWeReturnOk()
        {
            var request = this
                .GivenRequest();
            var handler = this
                .GivenCommandHandler<ArchiveFeatureCommand>()
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
                .GivenCommandHandler<ArchiveFeatureCommand>()
                .WithException<ArchiveFeatureCommand, TestException>();

            await this
                .GivenController(handler.Object)
                .WhenProcessingCommand(request)
                .ThenExceptionIsThrown<TestException, IActionResult>();
        }
    }

    public static class ArchiveFeatureControllerTestsExtensions
    {
        public static ArchiveFeatureRequest GivenRequest(this ArchiveFeatureControllerTests tests)
        {
            return new ArchiveFeatureRequest
            {
                ArchivedBy = "bob",
                Name = "Ross",
                Path = "🦄.🖼",
            };
        }

        public static ArchiveFeatureController GivenController(
            this ArchiveFeatureControllerTests tests, 
            IHandleCommand<ArchiveFeatureCommand> handler)
        {
            return new ArchiveFeatureController(handler);
        }

        public static Func<Task<IActionResult>> WhenProcessingCommand(
            this ArchiveFeatureController controller,
            ArchiveFeatureRequest request)
        {
            return () => controller.Post(request);
        }
    }
}