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

        [Route("{urlEncodedPath}/{urlEncodedName}")]
        public async Task<bool> Get(string urlEncodedPath, string urlEncodedName)
        {
            var path = WebUtility.UrlDecode(urlEncodedPath);
            var name = WebUtility.UrlDecode(urlEncodedName);

            path.Required(nameof(path));
            name.Required(nameof(name));
            var result = await this._handler.Handle(new IsFeatureOnQuery
            {
                Path = path,
                Name = name,
            });

            this._evaluationCounter.Inc(PathHelper.CombineNameAndPath(path, name), result);

            return result;
        }
    }
}
