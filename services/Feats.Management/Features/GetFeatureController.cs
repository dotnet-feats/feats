using System;
using System.Net;
using System.Threading.Tasks;
using Feats.CQRS.Queries;
using Feats.Management.Features.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Feats.Management.Features
{
    [ApiController]
    [Route("features")]
    public class GetFeatureController
    {
        private readonly IHandleQuery<GetFeatureQuery, FeatureAndStrategyConfiguration> _handler;

        public GetFeatureController(IHandleQuery<GetFeatureQuery, FeatureAndStrategyConfiguration> handler)
        {
            this._handler = handler;
        }

        public async Task<FeatureAndStrategyConfiguration> Get([FromQuery] string urlEncodedPath, [FromQuery] string urlEncodedName)
        {
            var path = WebUtility.UrlDecode(urlEncodedPath);
            var name = WebUtility.UrlDecode(urlEncodedName);

            return await this._handler.Handle(new GetFeatureQuery
            {
                Path = path,
                Name = name,
            });
        }
    }
}