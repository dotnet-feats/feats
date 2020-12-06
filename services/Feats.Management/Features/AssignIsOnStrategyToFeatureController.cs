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
    [Route("features/strategies")]
    public class AssignIsOnStrategyToFeatureController
    {
        private readonly IHandleCommand<AssignIsOnStrategyToFeatureCommand> _handleCommand;

        public AssignIsOnStrategyToFeatureController(IHandleCommand<AssignIsOnStrategyToFeatureCommand> handleCommand)
        {
            this._handleCommand = handleCommand;
        }

        [Route("ison")]
        public async Task<IActionResult> Post([FromBody] AssignIsOnStrategyToFeatureRequest request)
        {
            request.Validate();
            var command = request.ToAssignIsOnStrategyToFeatureCommand();

            await this._handleCommand.Handle(command);

            return new StatusCodeResult((int) HttpStatusCode.OK);
        }
    }

    public class AssignIsOnStrategyToFeatureRequest
    {
        public string Name { get; set; }

        public string Path { get; set; }
        
        public string AssignedBy { get; set; }
        
        public bool IsOn { get; set; }
    }

    public static class AssignIsOnStrategyToFeatureRequestExtensions
    {
        public static void Validate(this AssignIsOnStrategyToFeatureRequest request)
        {
            request.Required(nameof(request));
            request.Name.Required(nameof(request.Name));
            request.AssignedBy.Required(nameof(request.AssignedBy));
        } 

        public static AssignIsOnStrategyToFeatureCommand ToAssignIsOnStrategyToFeatureCommand(this AssignIsOnStrategyToFeatureRequest request)
        {
            return new AssignIsOnStrategyToFeatureCommand
            {
                Name = request.Name,
                Path = request.Path,
                AssignedBy = request.AssignedBy,
                IsEnabled = request.IsOn,
            };
        }   
    }
}
