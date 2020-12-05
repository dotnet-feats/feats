using System;
using Feats.CQRS.Queries;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Feats.Management.Features.Queries
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddQueries(this IServiceCollection services)
        {
            services.TryAddScoped<IHandleQuery<GetFeatureQuery, FeatureAndStrategyConfiguration>, GetFeatureQueryHandler>();

            return services;
        }
    }
}
