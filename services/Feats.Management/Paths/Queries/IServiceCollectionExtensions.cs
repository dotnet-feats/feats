using System;
using System.Collections.Generic;
using Feats.CQRS.Queries;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Feats.Management.Paths.Queries
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddQueries(this IServiceCollection services)
        {
            services.TryAddScoped<IHandleQuery<GetAllPathsQuery, IEnumerable<PathAndFeatureCount>>, GetAllPathsQueryHandler>();

            return services;
        }
    }
}
