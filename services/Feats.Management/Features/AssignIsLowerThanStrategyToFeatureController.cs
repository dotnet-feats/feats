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
    public class AssignIsLowerThanStrategyToFeatureController
    {
        private readonly IHandleCommand<AssignIsLowerThanStrategyToFeatureCommand> _handleCommand;

        public AssignIsLowerThanStrategyToFeatureController(IHandleCommand<AssignIsLowerThanStrategyToFeatureCommand> handleCommand)
        {
            this._handleCommand = handleCommand;
        }

        [Route("islower")]
        public async Task<IActionResult> Post([FromBody] AssignIsLowerThanStrategyToFeatureRequest request)
        {
            request.Validate();
            var command = request.ToAssignIsLowerThanStrategyToFeatureCommand();

            await this._handleCommand.Handle(command);

            return new StatusCodeResult((int) HttpStatusCode.OK);
        }
    }

    public class AssignIsLowerThanStrategyToFeatureRequest
    {
        public string Name { get; set; }

        public string Path { get; set; }
        
        public string AssignedBy { get; set; }
        
        public double? Value { get; set; }
    }

    public static class AssignIsLowerThanStrategyToFeatureRequestExtensions
    {
        public static void Validate(this AssignIsLowerThanStrategyToFeatureRequest request)
        {
            request.Required(nameof(request));
            request.Name.Required(nameof(request.Name));
            request.Value.Required(nameof(request.Value));
            request.AssignedBy.Required(nameof(request.AssignedBy));
        } 

        public static AssignIsLowerThanStrategyToFeatureCommand ToAssignIsLowerThanStrategyToFeatureCommand(this AssignIsLowerThanStrategyToFeatureRequest request)
        {
            return new AssignIsLowerThanStrategyToFeatureCommand
            {
                Name = request.Name,
                Path = request.Path,
                AssignedBy = request.AssignedBy,
                Value = request.Value.GetValueOrDefault()
            };
        }   
    }
}
