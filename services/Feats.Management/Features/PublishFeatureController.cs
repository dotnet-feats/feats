using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Feats.CQRS.Commands;
using Feats.Domain.Validations;
using Feats.Management.Features.Commands;
using Microsoft.AspNetCore.Mvc;

namespace Feats.Management.Features
{
    [ApiController]
    [Route("features")]
    public class PublishFeatureController
    {
        private readonly IHandleCommand<PublishFeatureCommand> _handleCommand;

        public PublishFeatureController(IHandleCommand<PublishFeatureCommand> handleCommand)
        {
            this._handleCommand = handleCommand;
        }

        [Route("publish")]
        public async Task<IActionResult> Post([FromBody] PublishFeatureRequest request)
        {
            request.Validate();
            var command = request.ToPublishFeatureCommand();

            await this._handleCommand.Handle(command);

            return new StatusCodeResult((int) HttpStatusCode.OK);
        }
    }

    public class PublishFeatureRequest
    {
        public string Name { get; set; }

        public string Path { get; set; }
        
        public string PublishedBy { get; set; }
    }

    public static class PublishFeatureRequestExtensions
    {
        public static void Validate(this PublishFeatureRequest request)
        {
            request.Required(nameof(request));
            request.Name.Required(nameof(request.Name));
            request.PublishedBy.Required(nameof(request.PublishedBy));
        } 

        public static PublishFeatureCommand ToPublishFeatureCommand(this PublishFeatureRequest request)
        {
            return new PublishFeatureCommand
            {
                Name = request.Name,
                Path = request.Path,
                PublishedBy = request.PublishedBy,
            };
        }   
    }
}
