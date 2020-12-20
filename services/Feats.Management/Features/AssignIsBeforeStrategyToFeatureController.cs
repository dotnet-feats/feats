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
    public class AssignIsBeforeStrategyToFeatureController
    {
        private readonly IHandleCommand<AssignIsBeforeStrategyToFeatureCommand> _handleCommand;

        public AssignIsBeforeStrategyToFeatureController(IHandleCommand<AssignIsBeforeStrategyToFeatureCommand> handleCommand)
        {
            this._handleCommand = handleCommand;
        }

        [Route("isbefore")]
        public async Task<IActionResult> Post([FromBody] AssignIsBeforeStrategyToFeatureRequest request)
        {
            request.Validate();
            var command = request.ToAssignIsBeforeStrategyToFeatureCommand();

            await this._handleCommand.Handle(command);

            return new StatusCodeResult((int) HttpStatusCode.OK);
        }
    }

    public class AssignIsBeforeStrategyToFeatureRequest
    {
        public string Name { get; set; }

        public string Path { get; set; }
        
        public string AssignedBy { get; set; }
        
        public DateTimeOffset? Value { get; set; }
    }

    public static class AssignIsBeforeStrategyToFeatureRequestExtensions
    {
        public static void Validate(this AssignIsBeforeStrategyToFeatureRequest request)
        {
            request.Required(nameof(request));
            request.Name.Required(nameof(request.Name));
            request.Value.Required(nameof(request.Value));
            request.AssignedBy.Required(nameof(request.AssignedBy));
        } 

        public static AssignIsBeforeStrategyToFeatureCommand ToAssignIsBeforeStrategyToFeatureCommand(this AssignIsBeforeStrategyToFeatureRequest request)
        {
            return new AssignIsBeforeStrategyToFeatureCommand
            {
                Name = request.Name,
                Path = request.Path,
                AssignedBy = request.AssignedBy,
                Value = request.Value.GetValueOrDefault()
            };
        }   
    }
}
