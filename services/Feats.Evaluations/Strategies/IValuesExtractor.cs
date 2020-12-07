
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace Feats.Evaluations.Strategies
{
    public interface IValuesExtractor
    {
        IDictionary<string, string> Extract();
    }

    public class ValuesExtractor : IValuesExtractor
    {
        private readonly IHttpContextAccessor _accessor;

        public ValuesExtractor(IHttpContextAccessor accessor)
        {
            this._accessor = accessor;
        }

        public IDictionary<string, string> Extract()
        {
            return this._accessor.HttpContext.Request.Headers
                .ToDictionary(hc => hc.Key, kc => kc.Value.ToString());
        }
    }
}