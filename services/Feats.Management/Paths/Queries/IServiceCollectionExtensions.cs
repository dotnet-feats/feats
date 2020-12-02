using System;
using Microsoft.Extensions.DependencyInjection;

namespace Feats.Management.Paths.Queries
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddQueries(this IServiceCollection services)
        {
            return services;
        }
    }
}
