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
    public class CreateFeatureController
    {
        private readonly IHandleCommand<CreateFeatureCommand> _handleCommand;

        public CreateFeatureController(IHandleCommand<CreateFeatureCommand> handleCommand)
        {
            this._handleCommand = handleCommand;
        }

        public async Task<IActionResult> Put([FromBody] CreateFeatureRequest createFeatureRequest)
        {
            createFeatureRequest.Validate();
            var command = createFeatureRequest.ToCreateFeatureCommand();

            await this._handleCommand.Handle(command);

            return new StatusCodeResult((int) HttpStatusCode.Created);
        }
    }

    public class CreateFeatureRequest
    {
        public string Name { get; set; }

        public string Path { get; set; }
        
        public string CreatedBy { get; set; }
    }

    public static class CreateFeatureRequestExtensions
    {
        public static void Validate(this CreateFeatureRequest createFeatureRequest)
        {
            createFeatureRequest.Required(nameof(createFeatureRequest));
            createFeatureRequest.Name.Required(nameof(createFeatureRequest.Name));
            createFeatureRequest.CreatedBy.Required(nameof(createFeatureRequest.CreatedBy));
        } 

        public static CreateFeatureCommand ToCreateFeatureCommand(this CreateFeatureRequest createFeatureRequest)
        {
            return new CreateFeatureCommand
            {
                Name = createFeatureRequest.Name,
                Path = createFeatureRequest.Path,
                CreatedBy = createFeatureRequest.CreatedBy,
            };
        }   
    }
}
