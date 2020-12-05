using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Feats.CQRS.Queries;
using Feats.Domain.Validations;
using Feats.Management.Features.Commands;
using Feats.Management.Paths.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Feats.Management.Features
{
    [ApiController]
    [Route("features")]
    public class GetAllPathsController
    {
        private readonly IHandleQuery<GetAllPathsQuery, IEnumerable<PathAndFeatureCount>> _handler;

        public GetAllPathsController(IHandleQuery<GetAllPathsQuery, IEnumerable<PathAndFeatureCount>> handler)
        {
            this._handler = handler;
        }

        public async Task<IEnumerable<PathAndFeatureCount>> Get([FromQuery] string filter)
        {
            return await this._handler.Handle(new GetAllPathsQuery
            {
                Filter = filter,
            });
        }
    }
}