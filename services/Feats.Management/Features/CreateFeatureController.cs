using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Feats.CQRS.Commands;
using Feats.Domain.Validations;
using Feats.Management.Features.Commands;
using Microsoft.AspNetCore.Mvc;

namespace Feats.Management.Features
{
    //[Authorize]
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
            createFeatureRequest.Required(nameof(createFeatureRequest));

            await this._handleCommand.Handle(new CreateFeatureCommand
            {
                Name = createFeatureRequest.Name,
                Path = createFeatureRequest.Path,
                CreatedBy = createFeatureRequest.CreatedBy,
                StrategyNames = createFeatureRequest.StrategyNames,
            });

            return new CreatedResult(string.Empty, null);
        }
    }

    public class CreateFeatureRequest
    {
        public string Name { get; set; }

        public string Path { get; set; }
        
        public string CreatedBy { get; set; }
        
        public IEnumerable<string> StrategyNames { get; set; }
    }
}
