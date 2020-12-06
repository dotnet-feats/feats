using System;
using Feats.Evaluations.Features.Metrics;
using Feats.Evaluations.Features.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace Feats.Evaluations.Features
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddFeatures(this IServiceCollection services)
        {
            services
                .AddMetrics()
                .AddQueries();
            
            return services;
        }
    }
}