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
    public class AssignIsGreaterThanStrategyToFeatureController
    {
        private readonly IHandleCommand<AssignIsGreaterThanStrategyToFeatureCommand> _handleCommand;

        public AssignIsGreaterThanStrategyToFeatureController(IHandleCommand<AssignIsGreaterThanStrategyToFeatureCommand> handleCommand)
        {
            this._handleCommand = handleCommand;
        }

        [Route("isgreater")]
        public async Task<IActionResult> Post([FromBody] AssignIsGreaterThanStrategyToFeatureRequest request)
        {
            request.Validate();
            var command = request.ToAssignIsGreaterThanStrategyToFeatureCommand();

            await this._handleCommand.Handle(command);

            return new StatusCodeResult((int) HttpStatusCode.OK);
        }
    }

    public class AssignIsGreaterThanStrategyToFeatureRequest
    {
        public string Name { get; set; }

        public string Path { get; set; }
        
        public string AssignedBy { get; set; }
        
        public double? Value { get; set; }
    }

    public static class AssignIsGreaterThanStrategyToFeatureRequestExtensions
    {
        public static void Validate(this AssignIsGreaterThanStrategyToFeatureRequest request)
        {
            request.Required(nameof(request));
            request.Name.Required(nameof(request.Name));
            request.Value.Required(nameof(request.Value));
            request.AssignedBy.Required(nameof(request.AssignedBy));
        } 

        public static AssignIsGreaterThanStrategyToFeatureCommand ToAssignIsGreaterThanStrategyToFeatureCommand(this AssignIsGreaterThanStrategyToFeatureRequest request)
        {
            return new AssignIsGreaterThanStrategyToFeatureCommand
            {
                Name = request.Name,
                Path = request.Path,
                AssignedBy = request.AssignedBy,
                Value = request.Value.GetValueOrDefault()
            };
        }   
    }
}
