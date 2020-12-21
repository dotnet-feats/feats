using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text.Encodings.Web;
using System.Web;

namespace Feats.Evaluation.Client
{
    public interface IFeatureEvaluationRequest
    {
        string Name { get; }
        
        string Path { get; }
        
        IEnumerable<KeyValuePair<string,string>> Strategies { get; }
    }

    internal sealed class FeatureEvaluationRequest : IFeatureEvaluationRequest
    {
        public FeatureEvaluationRequest()
        {
            this.Strategies = Enumerable.Empty<KeyValuePair<string, string>>();
        }

        public string Name { get; set; }
        
        public string Path { get; set; }
        
        public IEnumerable<KeyValuePair<string,string>> Strategies { get; set; }
    }

    internal static class IFeatureEvaluationRequestExtensions
    {
        internal static HttpRequestMessage ToHttpRequestMessage(
            this IFeatureEvaluationRequest request)
        {
            var encoder = UrlEncoder.Default;
            var builder = new UriBuilder();
            builder.Path = "features";
            var query = HttpUtility.ParseQueryString(builder.Query);
            query["name"] = encoder.Encode(request.Name);
            query["path"] = encoder.Encode(request.Path);
            builder.Query = query.ToString();
            
            var httpRequestMessage = new HttpRequestMessage(
                HttpMethod.Get,
                new Uri($"{builder.Path}?{builder.Query}", UriKind.Relative));
            
            foreach (var (key, value) in request.Strategies)
            {
                httpRequestMessage.Headers.TryAddWithoutValidation(key, value);
            }

            return httpRequestMessage;
        }

        internal static string GetCacheKey(this IFeatureEvaluationRequest request)
        {
            var strategies = request.Strategies.OrderBy(_ => _.Key).Select(_ => _.Key + _.Value);
            
            return $"${request.Name}${request.Path}${string.Join("_", strategies)}";
        }
    }
}