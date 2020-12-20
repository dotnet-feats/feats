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
    public class AssignIsInListStrategyToFeatureController
    {
        private readonly IHandleCommand<AssignIsInListStrategyToFeatureCommand> _handleCommand;

        public AssignIsInListStrategyToFeatureController(IHandleCommand<AssignIsInListStrategyToFeatureCommand> handleCommand)
        {
            this._handleCommand = handleCommand;
        }

        [Route("isinlist")]
        public async Task<IActionResult> Post([FromBody] AssignIsInListStrategyToFeatureRequest request)
        {
            request.Validate();
            var command = request.ToAssignIsInListStrategyToFeatureCommand();

            await this._handleCommand.Handle(command);

            return new StatusCodeResult((int) HttpStatusCode.OK);
        }
    }

    public class AssignIsInListStrategyToFeatureRequest
    {
        public AssignIsInListStrategyToFeatureRequest()
        {
            this.Items = Enumerable.Empty<string>();
        }
        
        public string Name { get; set; }

        public string Path { get; set; }
        
        public string AssignedBy { get; set; }
        
        public string ListName { get; set; }
        
        public IEnumerable<string> Items { get; set; }
    }

    public static class AssignIsInListStrategyToFeatureRequestExtensions
    {
        public static void Validate(this AssignIsInListStrategyToFeatureRequest request)
        {
            request.Required(nameof(request));
            request.Name.Required(nameof(request.Name));
            request.AssignedBy.Required(nameof(request.AssignedBy));
        } 

        public static AssignIsInListStrategyToFeatureCommand ToAssignIsInListStrategyToFeatureCommand(this AssignIsInListStrategyToFeatureRequest request)
        {
            return new AssignIsInListStrategyToFeatureCommand
            {
                Name = request.Name,
                Path = request.Path,
                AssignedBy = request.AssignedBy,
                Items = request.Items,
                ListName = request.ListName
            };
        }   
    }
}
