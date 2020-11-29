using System;
using Feats.Domain.Strategies;
using Feats.Management.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Feats.Evaluations.Features
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddFeatures(this IServiceCollection services)
        {
            services.TryAddSingleton<IFeaturesAggregate, FeaturesAggregate>();

            return services;
        }
    }
}