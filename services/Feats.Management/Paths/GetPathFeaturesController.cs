using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Feats.CQRS.Queries;
using Feats.Management.Paths.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Feats.Management.Paths
{
    [ApiController]
    [Route("paths")]
    public class GetPathFeaturesController
    {
        private readonly IHandleQuery<GetPathFeaturesQuery, IEnumerable<FeatureAndStrategy>> _handler;

        public GetPathFeaturesController(IHandleQuery<GetPathFeaturesQuery, IEnumerable<FeatureAndStrategy>> handler)
        {
            this._handler = handler;
        }

        [Route("features")]
        public async Task<IEnumerable<FeatureAndStrategy>> Get([FromQuery] string urlEncodedPath)
        {
            var path = WebUtility.UrlDecode(urlEncodedPath);

            return await this._handler.Handle(new GetPathFeaturesQuery
            {
                Path = path,
            });
        }
    }
}