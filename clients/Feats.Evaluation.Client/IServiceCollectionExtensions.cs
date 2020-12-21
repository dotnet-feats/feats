using System;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Polly;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Feats.Evaluation.Client
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddFeatsEvaluationClient(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services
                .AddHttpClient("evaluation")
                .AddTransientHttpErrorPolicy(builder => builder.WaitAndRetryAsync(new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(5),
                    TimeSpan.FromSeconds(10)
                }));
            
            services.AddLogging();
            services.TryAddSingleton<IEvaluationCache, EvaluationCache>();
            services.TryAddSingleton<IConfiguration>(configuration);
            services.TryAddSingleton<IFeatsEvaluationConfiguration, FeatsEvaluationConfiguration>();
            services.TryAddSingleton<IFeatsEvaluationClient, FeatsEvaluationClient>();
            services.TryAddSingleton<IFeatureEvaluationRequestBuilder, FeatureEvaluationRequestBuilder>();
            
            return services;
        }
    }
}