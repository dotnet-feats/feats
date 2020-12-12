using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Feats.Evaluations.Features.Metrics
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddMetrics(this IServiceCollection services)
        {
            services.TryAddSingleton<IEvaluationCounter, EvaluationCounter>();

            return services;
        }
    }
}
