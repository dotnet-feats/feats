using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace Feats.Evaluation.Client
{
    public interface IEvaluationCache
    {
        Task<bool> isOn(IFeatureEvaluationRequest request, Func<Task<bool>> isOnTask);
    }

    internal sealed class EvaluationCache : IEvaluationCache
    {
        private readonly IMemoryCache _cache;
        
        private readonly IFeatsEvaluationConfiguration _configuration;

        public EvaluationCache(
            IMemoryCache cache,
            IFeatsEvaluationConfiguration configuration)
        {
            this._cache = cache;
            this._configuration = configuration;
        }

        public async Task<bool> isOn(IFeatureEvaluationRequest request, Func<Task<bool>> isOnTask)
        {
            return await this._cache.GetOrCreateAsync(request.GetCacheKey(), entry =>
            {
                entry.SlidingExpiration = this._configuration.CacheTimeout;
                
                return isOnTask();
            });
        }
    }
}