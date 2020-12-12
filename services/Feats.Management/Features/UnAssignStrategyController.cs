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
    public class UnAssignStrategyController
    {
        private readonly IHandleCommand<UnAssignStrategyCommand> _handleCommand;

        public UnAssignStrategyController(IHandleCommand<UnAssignStrategyCommand> handleCommand)
        {
            this._handleCommand = handleCommand;
        }

        [HttpDelete]
        public async Task<IActionResult> Post([FromBody] UnAssignStrategyRequest request)
        {
            request.Validate();
            var command = request.ToUnAssignStrategyCommand();

            await this._handleCommand.Handle(command);

            return new StatusCodeResult((int) HttpStatusCode.OK);
        }
    }

    public class UnAssignStrategyRequest
    {
        public string Name { get; set; }

        public string Path { get; set; }
        
        public string UnassignedBy { get; set; }
        
        public string StrategyName { get; set; }
    }

    public static class UnAssignStrategyRequestExtensions
    {
        public static void Validate(this UnAssignStrategyRequest request)
        {
            request.Required(nameof(request));
            request.Name.Required(nameof(request.Name));
            request.StrategyName.Required(nameof(request.StrategyName));
            request.UnassignedBy.Required(nameof(request.UnassignedBy));
        } 

        public static UnAssignStrategyCommand ToUnAssignStrategyCommand(this UnAssignStrategyRequest request)
        {
            return new UnAssignStrategyCommand
            {
                Name = request.Name,
                Path = request.Path,
                UnassignedBy = request.UnassignedBy,
                StrategyName = request.StrategyName,
            };
        }   
    }
}
