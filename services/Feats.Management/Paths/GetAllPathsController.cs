using System.Collections.Generic;
using System.Threading.Tasks;
using Feats.CQRS.Queries;
using Feats.Management.Paths.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Feats.Management.Paths
{
    [ApiController]
    [Route("paths")]
    public class GetAllPathsController
    {
        private readonly IHandleQuery<GetAllPathsQuery, IEnumerable<PathAndFeatureCount>> _handler;

        public GetAllPathsController(IHandleQuery<GetAllPathsQuery, IEnumerable<PathAndFeatureCount>> handler)
        {
            this._handler = handler;
        }

        [HttpGet]
        public async Task<IEnumerable<PathAndFeatureCount>> Get([FromQuery] string path)
        {
            return await this._handler.Handle(new GetAllPathsQuery
            {
                Filter = path,
            });
        }
    }
}