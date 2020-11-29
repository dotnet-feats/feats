using System;
using System.Net;
using System.Threading.Tasks;
using Feats.CQRS.Queries;
using Feats.Domain.Validations;
using Feats.Evaluations.Features.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Feats.Evaluations.Features
{
    [ApiController]
    [Route("features")]
    public class IsOnController
    {
        private readonly IHandleQuery<IsFeatureOnQuery, bool> _handler;

        public IsOnController(IHandleQuery<IsFeatureOnQuery, bool> handdler)
        {
            this._handler = handdler;
        }

        [Route("{urlEncodedPath}/{urlEncodedName}")]
        public async Task<bool> Get(string urlEncodedPath, string urlEncodedName)
        {
            var path = WebUtility.UrlDecode(urlEncodedPath);
            var name = WebUtility.UrlDecode(urlEncodedName);

            path.Required(nameof(path));
            name.Required(nameof(name));

            return await this._handler.Handle(new IsFeatureOnQuery
            {
                Path = path,
                Name = name,
            });
        }
    }
}
