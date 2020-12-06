using System;
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
    public class UpdateFeatureController
    {
        private readonly IHandleCommand<UpdateFeatureCommand> _handleCommand;

        public UpdateFeatureController(IHandleCommand<UpdateFeatureCommand> handleCommand)
        {
            this._handleCommand = handleCommand;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] UpdateFeatureRequest createFeatureRequest)
        {
            createFeatureRequest.Validate();
            
            if (
                createFeatureRequest.NewName.Equals(createFeatureRequest.Name, StringComparison.InvariantCultureIgnoreCase) &&
                createFeatureRequest.NewPath.Equals(createFeatureRequest.Path, StringComparison.InvariantCultureIgnoreCase))
            {
                return new StatusCodeResult((int)HttpStatusCode.NoContent);
            }

            var command = createFeatureRequest.ToUpdateFeatureCommand();

            await this._handleCommand.Handle(command);

            return new StatusCodeResult((int)HttpStatusCode.OK);
        }
    }

    public class UpdateFeatureRequest
    {
        public string Name { get; set; }

        public string Path { get; set; }
        
        public string NewName { get; set; }

        public string NewPath { get; set; }
        
        public string UpdatedBy { get; set; }
    }

    public static class UpdateFeatureRequestExtensions
    {
        public static void Validate(this UpdateFeatureRequest createFeatureRequest)
        {
            createFeatureRequest.Required(nameof(createFeatureRequest));
            createFeatureRequest.Name.Required(nameof(createFeatureRequest.Name));
            createFeatureRequest.NewName.Required(nameof(createFeatureRequest.NewName));
            createFeatureRequest.UpdatedBy.Required(nameof(createFeatureRequest.UpdatedBy));
        }

        public static UpdateFeatureCommand ToUpdateFeatureCommand(this UpdateFeatureRequest createFeatureRequest)
        {
            return new UpdateFeatureCommand
            {
                Name = createFeatureRequest.Name,
                Path = createFeatureRequest.Path,
                NewName = createFeatureRequest.NewName,
                NewPath = createFeatureRequest.NewPath,
                UpdatedBy = createFeatureRequest.UpdatedBy,
            };
        }
    }
}
