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
    public class UnassignStrategyController
    {
        private readonly IHandleCommand<UnassignStrategyCommand> _handleCommand;

        public UnassignStrategyController(IHandleCommand<UnassignStrategyCommand> handleCommand)
        {
            this._handleCommand = handleCommand;
        }

        [HttpDelete]
        public async Task<IActionResult> Post([FromBody] UnassignStrategyRequest request)
        {
            request.Validate();
            var command = request.ToUnassignStrategyCommand();

            await this._handleCommand.Handle(command);

            return new StatusCodeResult((int) HttpStatusCode.OK);
        }
    }

    public class UnassignStrategyRequest
    {
        public string Name { get; set; }

        public string Path { get; set; }
        
        public string UnassignedBy { get; set; }
        
        public string StrategyName { get; set; }
    }

    public static class UnassignStrategyRequestExtensions
    {
        public static void Validate(this UnassignStrategyRequest request)
        {
            request.Required(nameof(request));
            request.Name.Required(nameof(request.Name));
            request.StrategyName.Required(nameof(request.StrategyName));
            request.UnassignedBy.Required(nameof(request.UnassignedBy));
        } 

        public static UnassignStrategyCommand ToUnassignStrategyCommand(this UnassignStrategyRequest request)
        {
            return new UnassignStrategyCommand
            {
                Name = request.Name,
                Path = request.Path,
                UnassignedBy = request.UnassignedBy,
                StrategyName = request.StrategyName,
            };
        }   
    }
}
