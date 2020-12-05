using System;
using System.Net;
using System.Threading.Tasks;
using Feats.CQRS.Queries;
using Feats.Domain;
using Feats.Domain.Validations;
using Feats.Evaluations.Features.Metrics;
using Feats.Evaluations.Features.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Feats.Evaluations.Features
{
    [ApiController]
    [Route("features")]
    public class IsOnController
    {
        private readonly IHandleQuery<IsFeatureOnQuery, bool> _handler;
        private readonly IEvaluationCounter _evaluationCounter;

        public IsOnController(
            IHandleQuery<IsFeatureOnQuery, bool> handdler,
            IEvaluationCounter evaluationCounter)
        {
            this._handler = handdler;
            this._evaluationCounter = evaluationCounter;
        }

        [HttpGet]
        public async Task<bool> Get([FromQuery] string path, [FromQuery] string name)
        {
            var safePath = WebUtility.UrlDecode(path);
            var safeName = WebUtility.UrlDecode(name);

            safePath.Required(nameof(safePath));
            safeName.Required(nameof(safeName));
            
            var result = await this._handler.Handle(new IsFeatureOnQuery
            {
                Path = safePath,
                Name = safeName,
            });

            this._evaluationCounter.Inc(PathHelper.CombineNameAndPath(safePath, safeName), result);

            return result;
        }
    }
}
