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

        [Route("{path}/{name}")]
        public async Task<bool> Get(string path, string name)
        {
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
