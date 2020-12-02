using System;
using Feats.Management.Paths.Commands;
using Feats.Management.Paths.Events;
using Feats.Management.Paths.Queries;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Feats.Management.Paths
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddPaths(this IServiceCollection services)
        {
            services
            .AddCommands()
            .AddEvents()
            .AddQueries();

            services.TryAddScoped<IPathsAggregate, PathsAggregate>();

            return services;
        }
    }
}
