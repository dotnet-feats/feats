using System;
using Feats.Management.Paths.Commands;
using Feats.Management.Paths.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace Feats.Management.Paths
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddPaths(this IServiceCollection services)
        {
            services
            .AddCommands()
            .AddQueries();

            return services;
        }
    }
}
