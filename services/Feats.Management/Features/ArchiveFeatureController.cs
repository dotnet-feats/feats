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
    public class ArchiveFeatureController
    {
        private readonly IHandleCommand<ArchiveFeatureCommand> _handleCommand;

        public ArchiveFeatureController(IHandleCommand<ArchiveFeatureCommand> handleCommand)
        {
            this._handleCommand = handleCommand;
        }

        [Route("archive")]
        public async Task<IActionResult> Post([FromBody] ArchiveFeatureRequest request)
        {
            request.Validate();
            var command = request.ToArchiveFeatureCommand();

            await this._handleCommand.Handle(command);

            return new StatusCodeResult((int) HttpStatusCode.OK);
        }
    }

    public class ArchiveFeatureRequest
    {
        public string Name { get; set; }

        public string Path { get; set; }
        
        public string ArchivedBy { get; set; }
    }

    public static class ArchiveFeatureRequestExtensions
    {
        public static void Validate(this ArchiveFeatureRequest request)
        {
            request.Required(nameof(request));
            request.Name.Required(nameof(request.Name));
            request.ArchivedBy.Required(nameof(request.ArchivedBy));
        } 

        public static ArchiveFeatureCommand ToArchiveFeatureCommand(this ArchiveFeatureRequest request)
        {
            return new ArchiveFeatureCommand
            {
                Name = request.Name,
                Path = request.Path,
                ArchivedBy = request.ArchivedBy,
            };
        }   
    }
}
