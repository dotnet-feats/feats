using System;
using Feats.CQRS.Queries;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Feats.Evaluations.Features.Queries
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddQueries(this IServiceCollection services)
        {
            services.TryAddScoped<IHandleQuery<IsFeatureOnQuery, bool>, IsFeatureOnQueryHandler>();

            return services;
        }
    }
}