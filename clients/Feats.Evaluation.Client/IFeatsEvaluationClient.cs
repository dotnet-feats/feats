using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Feats.Evaluation.Client
{
    public interface IFeatsEvaluationClient : IDisposable
    {
        Task<bool> IsOn(IFeatureEvaluationRequest request, CancellationToken token = default);
    }

    internal sealed class FeatsEvaluationClient : IFeatsEvaluationClient
    {
        private readonly ILogger<FeatsEvaluationClient> _logger;
        
        private readonly IEvaluationCache _cache;

        private readonly HttpClient _client;
        
        private readonly UrlEncoder _encoder;
        
        private readonly JsonSerializerOptions _jsonOptions;

        public FeatsEvaluationClient(
            IFeatsEvaluationConfiguration configuration,
            ILogger<FeatsEvaluationClient> logger,
            IHttpClientFactory httpClientFactory,
            IEvaluationCache cache)
        {
            this._logger = logger;
            this._cache = cache;
            this._encoder = UrlEncoder.Default;
            this._client = httpClientFactory.CreateClient("evaluations");
            this._client.BaseAddress = configuration.Host;
            this._client.Timeout = configuration.RequestTimeout;
            this._client.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));
            this._jsonOptions = new JsonSerializerOptions
            {
                Converters = { new JsonStringEnumConverter() }
            };
        }

        public async Task<bool> IsOn(IFeatureEvaluationRequest request, CancellationToken token = default)
        {
            if (!token.IsCancellationRequested)
            {
                return await this._cache.isOn(request, () => this.FetchIsOn(request, token));
            }
            
            throw new OperationCanceledException("Cancellation was requested while attempting to evaluate the cache for feature IsOn.");
        }

        public void Dispose()
        {
            this._client.Dispose();
        }
        
        private async Task<bool> FetchIsOn(IFeatureEvaluationRequest request, CancellationToken token = default)
        {
            var httpRequest = request.ToHttpRequestMessage(this._encoder);
            
            if (!token.IsCancellationRequested)
            {
                try
                {
                    var response = await this._client.SendAsync(httpRequest, token);

                    if (response.IsSuccessStatusCode)
                    {
                        return await response.Content.ReadFromJsonAsync<bool>(this._jsonOptions, token);
                    }

                    var content = await response.Content.ReadAsStringAsync(token);
                    var e = new FailedToCommunicateWithEvaluationsException(response, content);
                    throw e;
                }
                catch (Exception e)
                {
                    this._logger.LogError(e, $"An error occurred while requesting if feature was On ${e.Message}.");
                    throw;
                }
            } 
            
            throw new OperationCanceledException("Cancellation was requested while attempting to request feature IsOn.");
        }
    }
}

    