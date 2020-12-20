using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Feats.CQRS.Commands;
using Feats.Domain.Validations;
using Feats.Management.Features.Commands;
using Microsoft.AspNetCore.Mvc;

namespace Feats.Management.Features
{
    [ApiController]
    [Route("features/strategies")]
    public class AssignIsAfterStrategyToFeatureController
    {
        private readonly IHandleCommand<AssignIsAfterStrategyToFeatureCommand> _handleCommand;

        public AssignIsAfterStrategyToFeatureController(IHandleCommand<AssignIsAfterStrategyToFeatureCommand> handleCommand)
        {
            this._handleCommand = handleCommand;
        }

        [Route("isafter")]
        public async Task<IActionResult> Post([FromBody] AssignIsAfterStrategyToFeatureRequest request)
        {
            request.Validate();
            var command = request.ToAssignIsAfterStrategyToFeatureCommand();

            await this._handleCommand.Handle(command);

            return new StatusCodeResult((int) HttpStatusCode.OK);
        }
    }

    public class AssignIsAfterStrategyToFeatureRequest
    {
        public string Name { get; set; }

        public string Path { get; set; }
        
        public string AssignedBy { get; set; }
        
        public DateTimeOffset? Value { get; set; }
    }

    public static class AssignIsAfterStrategyToFeatureRequestExtensions
    {
        public static void Validate(this AssignIsAfterStrategyToFeatureRequest request)
        {
            request.Required(nameof(request));
            request.Name.Required(nameof(request.Name));
            request.Value.Required(nameof(request.Value));
            request.AssignedBy.Required(nameof(request.AssignedBy));
        } 

        public static AssignIsAfterStrategyToFeatureCommand ToAssignIsAfterStrategyToFeatureCommand(this AssignIsAfterStrategyToFeatureRequest request)
        {
            return new AssignIsAfterStrategyToFeatureCommand
            {
                Name = request.Name,
                Path = request.Path,
                AssignedBy = request.AssignedBy,
                Value = request.Value.GetValueOrDefault()
            };
        }   
    }
}
